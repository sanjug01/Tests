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
    public sealed class OrderedModelCollection<TModel>
        : MutableObject
        where TModel : class, INotifyPropertyChanged
    {
        private readonly IModelCollection<TModel> _sourceCollection;
        private readonly ObservableCollection<TModel> _orderedCollection;
        private readonly ReadOnlyObservableCollection<TModel> _models;
        private IComparer<TModel> _order;

        ReadOnlyObservableCollection<TModel> Models
        {
            get { return _models; }
        }

        public IComparer<TModel> Order
        {
            get { return _order; }

            set
            {
                if (this.SetProperty(ref _order, value))
                    RebuildCollection();
            }
        }

        public OrderedModelCollection(IModelCollection<TModel> sourceCollection)
        {
            _sourceCollection = sourceCollection;
            _orderedCollection = new ObservableCollection<TModel>();
            _models = new ReadOnlyObservableCollection<TModel>(_orderedCollection);

            INotifyCollectionChanged ncc = sourceCollection.Models;
            ncc.CollectionChanged += this.OnSourceCollectionChanged;
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (null != _order)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (IModelContainer<TModel> model in e.NewItems)
                        {
                            //
                            // Insert the new model at an appropriate position;
                            //
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (IModelContainer<TModel> model in e.NewItems)
                        {
                            //
                            // Insert the new model at an appropriate position;
                            //
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

        private void RebuildCollection()
        {
            List<TModel> models = new List<TModel>(_sourceCollection.Models.Count);

            foreach (IModelContainer<TModel> container in _sourceCollection.Models)
                models.Add(container.Model);

            if(null != _order)
                models.Sort(_order);

            _orderedCollection.Clear();

            foreach (TModel model in models)
                _orderedCollection.Add(model);
        }
    }
}
