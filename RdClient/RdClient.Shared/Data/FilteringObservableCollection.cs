namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public sealed class FilteringObservableCollection<TElement> : ReadOnlyObservableCollection<TElement>
    {
        private sealed class Filter
        {
            private readonly Predicate<TElement> _filterPredicate;
            private readonly ObservableCollection<TElement> _filteredSource;
            private readonly IEnumerable<TElement> _dataSource;

            public ObservableCollection<TElement> FilteredSource
            {
                get { return _filteredSource; }
            }

            public Filter(Predicate<TElement> filterPredicate, INotifyCollectionChanged collectionChanged, IEnumerable<TElement> dataSource)
            {
                _filterPredicate = filterPredicate;
                _filteredSource = new ObservableCollection<TElement>();
                _dataSource = dataSource;
                foreach(TElement e in dataSource)
                {
                    if (_filterPredicate(e))
                        _filteredSource.Add(e);
                }
                collectionChanged.CollectionChanged += this.OnSourceCollectionChanged;
            }

            private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                int index;

                switch(e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach(TElement newElement in e.NewItems)
                        {
                            if (_filterPredicate(newElement))
                                _filteredSource.Add(newElement);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach(TElement removedElement in e.OldItems)
                        {
                            if(_filterPredicate(removedElement))
                                _filteredSource.Remove(removedElement);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        index = 0;
                        foreach(TElement newElement in e.NewItems)
                        {
                            TElement replacedElement = (TElement)e.OldItems[index];

                            if (_filterPredicate(replacedElement))
                            {
                                int replacedIndex = _filteredSource.IndexOf(replacedElement);

                                if (_filterPredicate(newElement))
                                {
                                    _filteredSource[replacedIndex] = newElement;
                                }
                                else
                                {
                                    _filteredSource.RemoveAt(replacedIndex);
                                }
                            }
                            else
                            {
                                if (_filterPredicate(newElement))
                                {
                                    _filteredSource.Add(newElement);
                                }
                            }
                            ++index;
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _filteredSource.Clear();
                        foreach(TElement element in _dataSource)
                        {
                            if (_filterPredicate(element))
                                _filteredSource.Add(element);
                        }
                        break;
                }
            }
        }

        private readonly Filter _filter;

        public static ReadOnlyObservableCollection<TElement> Create<TWrappedCollection>(TWrappedCollection sourceCollection, Predicate<TElement> filterPredicate)
            where TWrappedCollection : INotifyCollectionChanged, IEnumerable<TElement>
        {
            Filter filter = new Filter(filterPredicate, sourceCollection, sourceCollection);

            return new FilteringObservableCollection<TElement>(filter);
        }

        private FilteringObservableCollection(Filter filter)
            : base(filter.FilteredSource)
        {
            _filter = filter;
        }
    }
}
