namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;

    public sealed class PrimaryModelCollection<TModel> : IModelCollection<TModel> where TModel : INotifyPropertyChanged
    {
        private static readonly string ModelFileExtension = "model";

        private readonly IStorageFolder _storageFolder;
        private readonly IModelSerializer _modelSerializer;
        private readonly ObservableCollection<IModelContainer<TModel>> _originalModels;
        private readonly ReadOnlyObservableCollection<IModelContainer<TModel>> _wrappedModels;
        private readonly IList<Guid> _removedModelIds;

        public static IModelCollection<TModel> Load(IStorageFolder folder, IModelSerializer modelSerializer)
        {
            Contract.Requires(null != folder);

            PrimaryModelCollection<TModel> loadedCollection = new PrimaryModelCollection<TModel>(folder, modelSerializer);

            loadedCollection.Load();

            return loadedCollection;
        }

        private PrimaryModelCollection(IStorageFolder folder, IModelSerializer modelSerializer)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != modelSerializer);
            Contract.Ensures(null != _storageFolder);
            Contract.Ensures(null != _modelSerializer);
            Contract.Ensures(null != _originalModels);
            Contract.Ensures(null != _wrappedModels);

            _storageFolder = folder;
            _modelSerializer = modelSerializer;
            _originalModels = new ObservableCollection<IModelContainer<TModel>>();
            _wrappedModels = new ReadOnlyObservableCollection<IModelContainer<TModel>>(_originalModels);
            _removedModelIds = new List<Guid>();
        }

        ReadOnlyObservableCollection<IModelContainer<TModel>> IModelCollection<TModel>.Models
        {
            get { return _wrappedModels; }
        }

        Guid IModelCollection<TModel>.AddNewModel(TModel newModel)
        {
            Contract.Requires(null != newModel);

            IModelContainer<TModel> container = ModelContainer<TModel>.CreateForNewModel(newModel);

            container.PropertyChanged += this.OnModelContainerPropertyChanged;
            _originalModels.Add(container);

            return container.Id;
        }

        TModel IModelCollection<TModel>.RemoveModel(Guid id)
        {
            TModel removedModel = default(TModel);

            int index = 0, count = _originalModels.Count;

            while (index < count)
            {
                IModelContainer<TModel> container = _originalModels[index];

                if (id.Equals(container.Id))
                {
                    container.PropertyChanged -= this.OnModelContainerPropertyChanged;
                    removedModel = container.Model;
                    _originalModels.RemoveAt(index);

                    if(ModelStatus.New != container.Status)
                    {
                        //
                        // The container was loaded from the storage folder; add the ID of the container
                        // to the list of deleted models.
                        //
                        _removedModelIds.Add(id);
                    }
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

        void IModelCollection<TModel>.Save()
        {
            //
            // Delete all deleted objects from the storage folder.
            //
            foreach (Guid removedModelId in _removedModelIds)
                _storageFolder.DeleteFile(MakeModelFileName(removedModelId));
            _removedModelIds.Clear();
            //
            // Go through all models and save the new and modified ones.
            //
            foreach (IModelContainer<TModel> modelContainer in _originalModels)
            {
                if (ModelStatus.Clean != modelContainer.Status)
                {
                    using(Stream stream = _storageFolder.CreateFile(MakeModelFileName(modelContainer.Id)))
                    {
                        if (null != stream)
                        {
                            _modelSerializer.WriteModel(modelContainer.Model, stream);
                            modelContainer.Status = ModelStatus.Clean;
                        }
                        else
                        {
                            //
                            // TODO: report an error
                            //
                        }
                    }
                }
            }
        }

        private void Load()
        {
            //
            // Load the collection from the folder passed to the constructor.
            //
            Contract.Assert(0 == _originalModels.Count);
            foreach(string fileName in _storageFolder.GetFiles())
            {
                string[] nameComponents = fileName.Split('.');
                //
                // If the file extension is .model, try to load a model object from the file.
                //
                if (2 == nameComponents.Length && string.Equals(ModelFileExtension, nameComponents[1]))
                {
                    Guid objectId;
                    //
                    // Name of the file must represent a valid GUID
                    //
                    if(Guid.TryParse(nameComponents[0], out objectId))
                    {
                        Stream fileData = _storageFolder.OpenFile(fileName);

                        if( null != fileData )
                        {
                            //
                            // Use the model serializer to load an object.
                            //
                            try
                            {
                                TModel model = _modelSerializer.ReadModel<TModel>(fileData);
                                IModelContainer<TModel> modelContainer = ModelContainer<TModel>.CreateForExistingModel(objectId, model);

                                modelContainer.PropertyChanged += this.OnModelContainerPropertyChanged;
                                _originalModels.Add(modelContainer);
                            }
                            catch(Exception ex)
                            {
                                //
                                // TODO: Report error.
                                //
                                Debug.WriteLine("Failed to load model|Type:{0}|{1}", typeof(TModel).Name, ex);
                            }
                        }
                    }
                }
            }
        }

        private static string MakeModelFileName(Guid id)
        {
            return string.Format("{0}.{1}", id, ModelFileExtension);
        }

        private void OnModelContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //
            // TODO: probably, add the sender to a collection of modified containers.
            //
        }
    }
}
