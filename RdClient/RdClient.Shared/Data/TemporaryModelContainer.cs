namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;

    public sealed class TemporaryModelContainer<TModel> : IModelContainer<TModel> where TModel : class, INotifyPropertyChanged
    {
        private readonly Guid _id;
        private readonly TModel _model;

        public TemporaryModelContainer(Guid id, TModel model)
        {
            _id = id;
            _model = model;
        }

        Guid IModelContainer<TModel>.Id
        {
            get { return _id; }
        }

        TModel IModelContainer<TModel>.Model
        {
            get { return _model; }
        }

        PersistentStatus IModelContainer<TModel>.Status
        {
            get
            {
                return PersistentStatus.Clean;
            }
            set
            {
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }
    }
}
