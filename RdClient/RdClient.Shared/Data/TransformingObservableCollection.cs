namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;

    public sealed class TransformingObservableCollection<TSource, TTransformed> : ReadOnlyObservableCollection<TTransformed>
    {
        private readonly ITransformator _transformator;

        private interface ITransformator
        {
            ObservableCollection<TTransformed> Transformed { get; }
        }

        private sealed class Transformator<TSourceCollection> : ITransformator where TSourceCollection : INotifyCollectionChanged, IEnumerable<TSource>
        {
            private readonly ObservableCollection<TTransformed> _transformed;
            private readonly Func<TSource, TTransformed> _transform;
            private readonly Action<TTransformed> _removed;

            public ObservableCollection<TTransformed> Transformed
            {
                get { return _transformed; }
            }

            public Transformator(TSourceCollection sourceCollection, Func<TSource, TTransformed> transform, Action<TTransformed> removed)
            {
                Contract.Requires(null != sourceCollection);
                Contract.Requires(null != transform);

                _transformed = new ObservableCollection<TTransformed>();
                _transform = transform;
                _removed = removed;

                foreach (TSource original in sourceCollection)
                {
                    _transformed.Add(_transform(original));
                }

                sourceCollection.CollectionChanged += this.OnSourceCollectionChanged;
            }

            private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                int index, count;

                switch(e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        foreach (TSource original in e.NewItems)
                            _transformed.Insert(index++, _transform(original));
                        break;

                    case NotifyCollectionChangedAction.Move:
                        count = e.OldItems.Count;

                        if( e.NewStartingIndex < e.OldStartingIndex)
                        {
                            int insertAt = e.NewStartingIndex;

                            for (index = 0; index < count; ++index)
                                _transformed.Move(e.OldStartingIndex, insertAt++);
                        }
                        else
                        {
                            for (index = 0; index < count; ++index)
                                _transformed.Move(e.OldStartingIndex, e.NewStartingIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        count = e.OldItems.Count;
                        for (index = 0; index < count; ++index)
                        {
                            TTransformed removedItem = _transformed[e.OldStartingIndex];
                            _transformed.RemoveAt(e.OldStartingIndex);
                            if (null != _removed)
                                _removed(removedItem);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        index = e.NewStartingIndex;
                        foreach (TSource original in e.NewItems)
                        {
                            TTransformed removedItem = _transformed[index];
                            _transformed[index++] = _transform(original);
                            if (null != _removed)
                                _removed(removedItem);
                        }
                        break;

                    default:
                        if (null != _removed)
                        {
                            foreach (TTransformed transformedItem in _transformed)
                                _removed(transformedItem);
                        }
                        _transformed.Clear();
                        foreach (TSource original in (IEnumerable<TSource>)sender)
                            _transformed.Add(_transform(original));
                        break;
                }
            }
        }

        public static ReadOnlyObservableCollection<TTransformed> Create<TWrappedCollection>(
            TWrappedCollection sourceCollection,
            Func<TSource, TTransformed> transform,
            Action<TTransformed> removed = null)
                where TWrappedCollection : INotifyCollectionChanged, IEnumerable<TSource>
        {
            Contract.Requires(null != sourceCollection);
            Contract.Requires(null != transform);

            Transformator<TWrappedCollection> transformator = new Transformator<TWrappedCollection>(sourceCollection, transform, removed);

            return new TransformingObservableCollection<TSource, TTransformed>(transformator);
        }

        private TransformingObservableCollection(ITransformator transformator)
            : base(transformator.Transformed)
        {
            _transformator = transformator;
        }
    }
}
