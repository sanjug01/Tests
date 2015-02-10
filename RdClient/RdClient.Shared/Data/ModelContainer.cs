namespace RdClient.Shared.Data
{
    using RdClient.Shared.Helpers;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    public sealed class ModelContainer<TModel> :
        MutableObject,
        IModelContainer<TModel> where TModel : class, IPersistentStatus
    {
        private readonly Guid _id;
        private readonly TModel _model;
        private PersistentStatus _status;

        Guid  IModelContainer<TModel>.Id
        {
            get { return _id; }
        }

        TModel IModelContainer<TModel>.Model
        {
            get
            {
                Contract.Ensures(null != Contract.Result<TModel>());
                return _model;
            }
        }

        public PersistentStatus Status
        {
            get { return _status; }

            private set
            {
                this.SetProperty(ref _status, value);
            }
        }

        void IPersistentStatus.SetClean()
        {
            _model.SetClean();
            _status = PersistentStatus.Clean;
        }

        public static IModelContainer<TModel> CreateForNewModel(TModel model)
        {
            Contract.Requires(null != model);
            Contract.Ensures(null != Contract.Result<ModelContainer<TModel>>());
            return new ModelContainer<TModel>(Guid.NewGuid(), model, PersistentStatus.New);
        }

        public static IModelContainer<TModel> CreateForExistingModel(Guid id, TModel model)
        {
            Contract.Requires(null != model);
            Contract.Ensures(null != Contract.Result<ModelContainer<TModel>>());
            return new ModelContainer<TModel>(id, model, PersistentStatus.Clean);
        }

        private ModelContainer(Guid id, TModel model, PersistentStatus status)
        {
            Contract.Requires(null != model);
            Contract.Ensures(null != _model);
            _id = id;
            _status = status;
            _model = model;
            _model.PropertyChanged += this.OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status") && PersistentStatus.Clean == _status)
            {
                this.Status = PersistentStatus.Modified;
            }
        }
    }
}
