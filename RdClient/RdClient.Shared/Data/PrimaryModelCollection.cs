namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;

    public sealed class PrimaryModelCollection<TModel> : IModelCollection<TModel> where TModel : INotifyPropertyChanged
    {
        private readonly IStorageFolder _storageFolder;
        private readonly ObservableCollection<IModelContainer<TModel>> _originalModels;
        private readonly ReadOnlyObservableCollection<IModelContainer<TModel>> _wrappedModels;

        public static IModelCollection<TModel> Load(IStorageFolder folder, IModelSerializer modelSerializer)
        {
            Contract.Requires(null != folder);

            PrimaryModelCollection<TModel> loadedCollection = new PrimaryModelCollection<TModel>(folder);

            loadedCollection.Load(modelSerializer);

            return loadedCollection;
        }

        private PrimaryModelCollection(IStorageFolder folder)
        {
            Contract.Requires(null != folder);
            Contract.Ensures(null != _storageFolder);
            Contract.Ensures(null != _originalModels);
            Contract.Ensures(null != _wrappedModels);

            _storageFolder = folder;
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

        private void Load(IModelSerializer serializer)
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
                if (2 == nameComponents.Length && string.Equals("model", nameComponents[1]))
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
                                TModel model = serializer.ReadModel<TModel>(fileData);
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
    }
}
