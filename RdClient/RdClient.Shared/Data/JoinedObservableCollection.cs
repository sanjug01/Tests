namespace RdClient.Shared.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Read only observable collection that combines unique elements from a set of other observable
    /// collections.
    /// </summary>
    /// <typeparam name="TElement">Type of elements of the collection.</typeparam>
    /// <remarks>All elements of the collection must be unique at all times. Having the same element in
    /// more than one of the joined collections or in one of the jpined collections will result
    /// in unexpected behavior.</remarks>
    public sealed class JoinedObservableCollection<TElement> : ReadOnlyObservableCollection<TElement>
    {
        private readonly ObservableCollection<TElement> _source;
        private readonly IList<INotifyCollectionChanged> _collections;

        /// <summary>
        /// Construct a new instance of JoinedObservableCollection.
        /// </summary>
        /// <returns>New instance of JoinedObservableCollection that isn't tracking any other collections.</returns>
        public static JoinedObservableCollection<TElement> Create()
        {
            return new JoinedObservableCollection<TElement>(new ObservableCollection<TElement>());
        }

        /// <summary>
        /// Add a new collection to the combined collection.
        /// </summary>
        /// <typeparam name="TCollection">Type of the added collection. The added collection must be enumerable and observable.</typeparam>
        /// <param name="collection">Collection to add and track.</param>
        public void AddCollection<TCollection>(TCollection collection)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
        {
            Contract.Assert(null != collection);

            _collections.Add(collection);

            foreach(TElement e in collection)
                _source.Add(e);

            collection.CollectionChanged += this.OnCollectionChanged;
        }

        private JoinedObservableCollection(ObservableCollection<TElement> source)
            : base(source)
        {
            _source = source;
            _collections = new List<INotifyCollectionChanged>();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int[] replacedIndexes;
            int i;

            switch( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (TElement element in e.NewItems)
                        _source.Add(element);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (TElement element in e.OldItems)
                        _source.Remove(element);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    replacedIndexes = new int[e.OldItems.Count];
                    i = 0;
                    foreach (TElement element in e.OldItems)
                        replacedIndexes[i++] = _source.IndexOf(element);
                    i = 0;
                    foreach (TElement element in e.NewItems)
                        _source[replacedIndexes[i++]] = element;
                    break;

                case NotifyCollectionChangedAction.Reset:
                    //
                    // Simply rebuild the entire collection. Not ideal, but eliminates tracking of the original source
                    // of each item.
                    //
                    _source.Clear();
                    foreach(IEnumerable<TElement> c in _collections)
                    {
                        foreach (TElement element in c)
                            _source.Add(element);
                    }
                    break;
            }
        }
    }
}
