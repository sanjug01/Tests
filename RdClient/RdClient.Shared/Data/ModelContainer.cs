namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    public sealed class ModelContainer<TModel> : IModelContainer<TModel> where TModel : INotifyPropertyChanged
    {
        private readonly Guid _id;
        private readonly TModel _model;
        private ModelStatus _status;

        Guid  IModelContainer<TModel>.Id
        {
            get
            {
                return _id;
            }
        }

        TModel IModelContainer<TModel>.Model
        {
            get
            {
                Contract.Ensures(null != Contract.Result<TModel>());
                return _model;
            }
        }

        ModelStatus IModelContainer<TModel>.Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    switch (_status)
                    {
                        case ModelStatus.Clean:
                            throw new InvalidOperationException("Cannot change status from Clean");

                        default:
                            if (ModelStatus.Clean != value)
                                throw new ArgumentException("Cannot change status to anything but Clean");
                            break;
                    }
                    _status = value;
                }
            }
        }

        public static IModelContainer<TModel> CreateForNewModel(TModel model)
        {
            Contract.Requires(null != model);
            Contract.Ensures(null != Contract.Result<ModelContainer<TModel>>());
            return new ModelContainer<TModel>(Guid.NewGuid(), model, ModelStatus.New);
        }

        public static IModelContainer<TModel> CreateForExistingModel(Guid id, TModel model)
        {
            Contract.Requires(null != model);
            Contract.Ensures(null != Contract.Result<ModelContainer<TModel>>());
            return new ModelContainer<TModel>(id, model, ModelStatus.Clean);
        }

        private ModelContainer(Guid id, TModel model, ModelStatus status)
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
            if (ModelStatus.Clean == _status)
                _status = ModelStatus.Modified;
        }
    }
}
