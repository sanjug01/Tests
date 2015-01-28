namespace RdClient.Shared.Data
{
    using RdClient.Shared.Helpers;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Observable collection of model objects that may be ordered.
    /// </summary>
    public sealed class OrderedObservableCollection<TModel> : MutableObject
        where TModel : class, INotifyPropertyChanged
    {
        private readonly IList<TModel> _sourceCollection;
        private readonly ObservableCollection<TModel> _orderedCollection;
        private readonly ReadOnlyObservableCollection<TModel> _models;
        private IComparer<TModel> _order;

        public ReadOnlyObservableCollection<TModel> Models
        {
            get { return _models; }
        }

        public IComparer<TModel> Order
        {
            get { return _order; }

            set
            {
                if (this.SetProperty(ref _order, value))
                {
                    RebuildCollection();
                }
            }
        }

        public OrderedObservableCollection(ReadOnlyObservableCollection<TModel> sourceCollection)
        {
            _sourceCollection = sourceCollection;
            _orderedCollection = new ObservableCollection<TModel>();
            _models = new ReadOnlyObservableCollection<TModel>(_orderedCollection);
            SubscribeForCollectionUpdates(sourceCollection);
        }

        public OrderedObservableCollection(ObservableCollection<TModel> sourceCollection)
        {
            _sourceCollection = sourceCollection;
            _orderedCollection = new ObservableCollection<TModel>();
            _models = new ReadOnlyObservableCollection<TModel>(_orderedCollection);
            SubscribeForCollectionUpdates(sourceCollection);
        }

        private void SubscribeForCollectionUpdates(INotifyCollectionChanged sourceCollection)
        {
            sourceCollection.CollectionChanged += this.OnSourceCollectionChanged;
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (null != _order)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (TModel model in e.NewItems)
                        {
                            InsertModelIntoOrderedCollection(model);
                            model.PropertyChanged += this.OnModelPropertyChanged;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (TModel model in e.OldItems)
                        {
                            //
                            // Remove the model from the ordered collection; first, find the index of the first model in the collection
                            // greater or equal to the removed model, then scan from that index up to the end of the collection
                            // looking for the removed model (comparison may not be unique).
                            //
                            int modelIndex = _orderedCollection.IndexOfFirstGreaterOrEqual(model, _order);

                            if (modelIndex >= 0)
                            {
                                while(0 == _order.Compare(model, _orderedCollection[modelIndex]))
                                {
                                    if(object.ReferenceEquals(model, _orderedCollection[modelIndex]))
                                    {
                                        _orderedCollection[modelIndex].PropertyChanged -= this.OnModelPropertyChanged;
                                        _orderedCollection.RemoveAt(modelIndex);
                                        break;
                                    }
                                    else
                                    {
                                        ++modelIndex;
                                        Contract.Assert(modelIndex < _orderedCollection.Count);
                                    }
                                }
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        //
                        // There's nothing to do, moving elements in the original collection does not affect their order.
                        //
                        break;

                    default:
                        RebuildCollection();
                        break;
                }
            }
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Contract.Assert(null != _order);
            //
            // Remove the changed model and re-insert it at the new appropriate position.
            //
            TModel model = (TModel)sender;

            int index = _orderedCollection.IndexOf(model);
            Contract.Assert(index >= 0);
            //
            // If the new position of the model is different (it is greater than the next one or less than the previous one)
            // remove and re-insert it at the new position.
            //
            bool mustMove = false;

            if (index < _orderedCollection.Count - 1 && _order.Compare(model, _orderedCollection[index + 1]) > 0)
                mustMove = true;

            if (!mustMove)
            {
                if (index > 0 && _order.Compare(model, _orderedCollection[index - 1]) < 0)
                    mustMove = true;
            }

            if(mustMove)
            {
                bool removed = _orderedCollection.Remove(model);
                Contract.Assert(removed);
                InsertModelIntoOrderedCollection(model);
            }
        }

        private void InsertModelIntoOrderedCollection(TModel model)
        {
            //
            // Insert the new model at an appropriate position;
            //
            int insertionPosition = _orderedCollection.IndexOfFirstGreaterOrEqual(model, _order);

            if (insertionPosition < 0)
                _orderedCollection.Add(model);
            else
                _orderedCollection.Insert(insertionPosition, model);
        }

        private void RebuildCollection()
        {
            if (null == _order)
            {
                foreach (TModel model in _orderedCollection)
                    model.PropertyChanged -= this.OnModelPropertyChanged;
                _orderedCollection.Clear();
            }
            else
            {
                List<TModel> models = new List<TModel>(_sourceCollection);

                models.Sort(_order);

                foreach (TModel model in _orderedCollection)
                    model.PropertyChanged -= this.OnModelPropertyChanged;
                _orderedCollection.Clear();

                foreach (TModel model in models)
                {
                    _orderedCollection.Add(model);
                    model.PropertyChanged += this.OnModelPropertyChanged;
                }
            }
        }
    }
}
