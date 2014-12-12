namespace RdClient.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Search;

    public sealed class FileSystemDataStorage : IDataStorage
    {
        private string _rootFolder;

        public sealed class SerializationContext : DisposableObject
        {
            private readonly DataContractSerializer _modelSerializer;
            private readonly StorageFolder _rootFolder;

            public SerializationContext(string rootFolderName)
            {
                Contract.Requires(null != rootFolderName);
                Contract.Ensures(null != _modelSerializer);
                Contract.Ensures(null != _rootFolder);
                _modelSerializer = new DataContractSerializer(typeof(ModelBase));
                _rootFolder = CreateFolder(rootFolderName, ApplicationData.Current.LocalFolder);
                Contract.Assert(null != _rootFolder);
            }

            public void WriteDataModel(RdDataModel dataModel)
            {
                Contract.Requires(null != dataModel);
            }

            public void ReadDataModel(RdDataModel dataModel)
            {
                Contract.Requires(null != dataModel);
            }

            public StorageFolder CreateRootFolder(string folderName)
            {
                Contract.Requires(null != folderName);
                return CreateFolder(folderName, _rootFolder);
            }

            public StorageFolder CreateChildFolder(string folderName, StorageFolder parentFolder)
            {
                Contract.Requires(null != folderName);
                Contract.Requires(null != parentFolder);
                return CreateFolder(folderName, parentFolder);
            }

            public T ReadModelObject<T>(StorageFolder folder, string fileName) where T : ModelBase
            {
                T loadedObject = null;

                using(Stream stream = OpenReadStream(fileName, folder))
                {
                    loadedObject = _modelSerializer.ReadObject(stream) as T;
                }

                if (null == loadedObject)
                    throw new InvalidDataException(string.Format("Could not load object of type {0} from file \"{1}\"", typeof(T).Name, fileName));

                return loadedObject;
            }

            public void WriteModelObject<T>(T modelObject, StorageFolder folder, string fileName) where T : ModelBase
            {
                using(Stream stream = CreateWriteStream(fileName, folder))
                {
                    _modelSerializer.WriteObject(stream, modelObject);
                    stream.Flush();
                }
            }

            public void ReadModelCollection<T>(ModelCollection<T> collection, StorageFolder folder, string collectionName) where T : ModelBase
            {
                Contract.Requires(null != collection);
                Contract.Requires(null != folder);
                Contract.Requires(null != collectionName);
                //
                // List all files of type .object in the collection name folder, load objects from all files
                // and add them to the collection.
                //
                StorageFolder collectionFolder = CreateFolder(collectionName, folder);

                IList<string> fileTypeFilter = new List<string>();
                fileTypeFilter.Add(".object");
                QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);

                // Create query and retrieve files
                StorageFileQueryResult query = collectionFolder.CreateFileQueryWithOptions(queryOptions);
                Task<IReadOnlyList<StorageFile>> fileListTask = query.GetFilesAsync().AsTask<IReadOnlyList<StorageFile>>();
                fileListTask.Wait();

                collection.Clear();

                foreach (StorageFile file in fileListTask.Result)
                {
                    Task<Stream> streamTask = file.OpenStreamForReadAsync();
                    streamTask.Wait();
                    T loadedObject = _modelSerializer.ReadObject(streamTask.Result) as T;
                    streamTask.Result.Dispose();

                    if (null == loadedObject)
                    {
                        throw new InvalidDataException(string.Format("Failed to load object of type {0} for collection {1}",
                            typeof(T), collectionName));
                    }

                    collection.Add(loadedObject);
                }
            }

            public void WriteModelCollection<T>(ModelCollection<T> collection, StorageFolder folder, string collectionName) where T : ModelBase
            {
                Contract.Requires(null != collection);
                Contract.Requires(null != folder);
                Contract.Requires(null != collectionName);
                //
                // Create a folder with the specified collection name and write individual objects in files of type .object
                // named sequentially.
                // When a collection is loaded, the list of files in the folders is queried ordered by name so the original
                // order of elements in the colection is preserved.
                //
                StorageFolder collectionFolder = CreateFolder(collectionName, folder);
                int index = 0;

                foreach(T model in collection)
                {
                    WriteModelObject(model, collectionFolder, string.Format("{0}.object", (index++).ToString("D12")));
                }
            }

            private static StorageFolder CreateFolder(string folderName, StorageFolder parentFolder)
            {
                Contract.Requires(null != folderName);
                Contract.Ensures(null != Contract.Result<StorageFolder>());
                Contract.Assert(null != parentFolder);

                Task<StorageFolder> folderTask = parentFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).AsTask<StorageFolder>();
                folderTask.Wait();
                Contract.Assert(null != folderTask.Result);
                return folderTask.Result;
            }

            private static Stream OpenReadStream(string fileName, StorageFolder folder)
            {
                Task<Stream> task = folder.OpenStreamForReadAsync(fileName);
                task.Wait();
                return task.Result;
            }

            private static Stream CreateWriteStream(string fileName, StorageFolder folder)
            {
                Task<Stream> task = folder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting);
                task.Wait();
                return task.Result;
            }

            private void LoadWorkspace(Workspace workspace, StorageFolder parentFolder, string workspaceFolderName)
            {
                Contract.Requires(null != workspace);
                Contract.Requires(null != parentFolder);
                Contract.Requires(null != workspaceFolderName);

                StorageFolder workspaceFolder = CreateFolder(workspaceFolderName, parentFolder);
                Workspace loadedWorkspace = ReadModelObject<Workspace>(workspaceFolder, ".workspace");
            }
        }

        public FileSystemDataStorage()
        {
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                //
                // TODO: add validation if necessary; doesn't look necessary at the moment.
                //
                _rootFolder = value;
            }
        }

        void IDataStorage.Load(RdDataModel persistentData)
        {
            Contract.Requires(null != persistentData);
            Contract.Assert(null != _rootFolder);

            using(SerializationContext sctx = new SerializationContext(_rootFolder))
            {
                sctx.ReadDataModel(persistentData);
            }
        }

        void IDataStorage.Save(RdDataModel persistentData)
        {
            Contract.Requires(null != persistentData);
            Contract.Assert(null != _rootFolder);

            using (SerializationContext sctx = new SerializationContext(_rootFolder))
            {
                sctx.WriteDataModel(persistentData);
            }
        }
    }
}
