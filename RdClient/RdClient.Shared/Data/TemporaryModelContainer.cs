namespace RdClient.Shared.Data
{
    using RdClient.Shared.Helpers;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A model container that temporarily wraps a model object and gives it any ID.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public sealed class TemporaryModelContainer<TModel> : MutableObject, IModelContainer<TModel> where TModel : class, IPersistentStatus
    {
        private readonly Guid _id;
        private readonly TModel _model;
        private PersistentStatus _status;

        /// <summary>
        /// Cast the model to a related type and wrap the cast object in a model container, giving it an ID.
        /// </summary>
        /// <typeparam name="TBaseModel">Related model type; in most cases - a parent type of TModel.</typeparam>
        /// <param name="id">Identifier of the wrapped model in the new container.</param>
        /// <param name="model">Model object to wrap in the container. Canot be null.</param>
        /// <returns>IModelContainer that wraps the object case to TModel with the specified ID.</returns>
        public static IModelContainer<TModel> WrapModel<TBaseModel>(Guid id, TBaseModel model) where TBaseModel : class, IPersistentStatus
        {
            Contract.Assert(null != model, "Model object must be non-null");
            Contract.Assert(model is TModel);
            return new TemporaryModelContainer<TModel>(id, model as TModel);
        }

        public static IModelContainer<TModel> WrapModel<TBaseModel>(IModelContainer<TBaseModel> container) where TBaseModel : class, IPersistentStatus
        {
            Contract.Assert(null != container);
            return WrapModel<TBaseModel>(container.Id, container.Model);
        }

        protected override void DisposeManagedState()
        {
            _model.PropertyChanged -= this.OnModelPropertyChanged;
        }

        private TemporaryModelContainer(Guid id, TModel model)
        {
            Contract.Requires(null != model, "Model object must be non-null");
            Contract.Ensures(null != _model);

            _id = id;
            _model = model;
            _status = PersistentStatus.Clean;
            _model.PropertyChanged += this.OnModelPropertyChanged;
        }

        Guid IModelContainer<TModel>.Id
        {
            get { return _id; }
        }

        TModel IModelContainer<TModel>.Model
        {
            get { return _model; }
        }

        public PersistentStatus Status
        {
            get
            {
                return _status;
            }

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

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //
            // Mark the model "Modified"
            //
            this.Status = PersistentStatus.Modified;
        }
    }
}
