namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    public sealed class PrimaryModelCollection<TModel> : IModelCollection<TModel> where TModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<IModelContainer<TModel>> _originalModels;
        private readonly ReadOnlyObservableCollection<IModelContainer<TModel>> _wrappedModels;

        public PrimaryModelCollection()
        {
            _originalModels = new ObservableCollection<IModelContainer<TModel>>();
            _wrappedModels = new ReadOnlyObservableCollection<IModelContainer<TModel>>(_originalModels);
        }

        ReadOnlyObservableCollection<IModelContainer<TModel>> IModelCollection<TModel>.Models
        {
            get { return _wrappedModels; }
        }

        Guid IModelCollection<TModel>.AddNewModel(TModel newModel)
        {
            Contract.Requires(null != newModel);

            IModelContainer<TModel> container = ModelContainer<TModel>.CreateForNewModel(newModel);

            _originalModels.Add(container);
            return container.Id;
        }

        TModel IModelCollection<TModel>.RemoveModel(Guid id)
        {
            TModel removedModel = default(TModel);

            int index = 0, count = _originalModels.Count;

            while (index < count)
            {
                if (id.Equals(_originalModels[index].Id))
                {
                    removedModel = _originalModels[index].Model;
                    _originalModels.RemoveAt(index);
                    break;
                }
                else
                {
                    ++index;
                }
            }

            if (index == count)
                throw new InvalidOperationException(string.Format("Model {0} was not found.", id));

            return removedModel;
        }
    }
}
