namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public sealed class TransformingObservableCollection<TSource, TTransformed> : ReadOnlyObservableCollection<TTransformed>
    {
        private readonly Transformator _transformator;

        private sealed class Transformator
        {
            private readonly ObservableCollection<TTransformed> _transformed;
            private readonly Func<TSource, TTransformed> _transformation;

            public ObservableCollection<TTransformed> Transformed
            {
                get { return _transformed; }
            }

            public Transformator(ReadOnlyObservableCollection<TSource> sourceCollection, Func<TSource, TTransformed> transformation)
            {
                _transformed = new ObservableCollection<TTransformed>();
                _transformation = transformation;

                foreach (TSource original in sourceCollection)
                {
                    _transformed.Add(transformation(original));
                }

                INotifyCollectionChanged ncc = sourceCollection;
                ncc.CollectionChanged += this.OnSourceCollectionChanged;
            }

            private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                int index, count;

                switch(e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        foreach (TSource original in e.NewItems)
                            _transformed.Insert(index++, _transformation(original));
                        break;

                    case NotifyCollectionChangedAction.Move:
                        count = e.OldItems.Count;
                        for (index = 0; index < count; ++index)
                            _transformed.Move(e.NewStartingIndex + index, e.OldStartingIndex + index);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        count = e.OldItems.Count;
                        for (index = 0; index < count; ++index)
                            _transformed.RemoveAt(e.OldStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        index = e.NewStartingIndex;
                        foreach (TSource original in e.NewItems)
                            _transformed[index++] = _transformation(original);
                        break;

                    default:
                        _transformed.Clear();
                        foreach (TSource original in (IEnumerable<TSource>)sender)
                            _transformed.Add(_transformation(original));
                        break;
                }
            }
        }

        public static ReadOnlyObservableCollection<TTransformed> Create(
            ReadOnlyObservableCollection<TSource> sourceCollection,
            Func<TSource, TTransformed> transformation)
        {
            Transformator transformator = new Transformator(sourceCollection, transformation);

            return new TransformingObservableCollection<TSource, TTransformed>(transformator);
        }

        private TransformingObservableCollection(Transformator transformator)
            : base(transformator.Transformed)
        {
            _transformator = transformator;
        }
    }
}
