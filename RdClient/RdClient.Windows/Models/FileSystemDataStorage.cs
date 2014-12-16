namespace RdClient.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization;
    using Windows.Storage;
    using Windows.Storage.Search;

    public sealed class FileSystemDataStorage : IDataStorage
    {
        private string _rootFolder;

        private sealed class SerializationContext : DisposableObject
        {
            private readonly DataContractSerializer _modelSerializer;
            private readonly StorageFolder _rootFolder;

            public SerializationContext(string rootFolderName)
            {
                Contract.Requires(null != rootFolderName);
                Contract.Ensures(null != _modelSerializer);
                Contract.Ensures(null != _rootFolder);
                StorageFolder root = null;

                _modelSerializer = new DataContractSerializer(typeof(ModelBase));
                ApplicationData.Current.LocalFolder.GetOrCreateFolderAndCall(rootFolderName, folder=> root = folder);
                _rootFolder = root;
                Contract.Assert(null != _rootFolder);
            }

            public void WriteDataModel(RdDataModel dataModel)
            {
                Contract.Requires(null != dataModel);
                //
                // Write the local workspace in the "local.workspace" folder
                //
                _rootFolder.CreateFolderAndCall("local.workspace", workspaceFolder =>
                    WriteWorkspace(dataModel.LocalWorkspace, workspaceFolder));
            }

            public void ReadDataModel(RdDataModel dataModel)
            {
                Contract.Requires(null != dataModel);
                //
                // Read the local workspace from the "local.workspace" folder
                //
                _rootFolder.GetFolderAndCall("local.workspace", workspaceFolder =>
                    ReadWorkspace(dataModel.LocalWorkspace, workspaceFolder));
            }

            public T ReadModelObject<T>(StorageFolder folder, string fileName) where T : ModelBase
            {
                T loadedObject = null;

                folder.OpenReadStreamAndCall(fileName, stream =>
                {
                    loadedObject = _modelSerializer.ReadObject(stream) as T;
                });

                if (null == loadedObject)
                    throw new InvalidDataException(string.Format("Could not load object of type {0} from file \"{1}\"", typeof(T).Name, fileName));

                return loadedObject;
            }

            public void ReadModelCollection<T>(ModelCollection<T> collection, StorageFolder folder) where T : ModelBase
            {
                Contract.Requires(null != collection);
                Contract.Requires(null != folder);

                QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new string[] { ".object" });

                collection.Clear();

                folder.ForEachFileCall(queryOptions, file =>
                    file.OpenReadStreamAndCall(stream =>
                    {
                        T loadedObject = _modelSerializer.ReadObject(stream) as T;
                        if(null == loadedObject)
                            throw new InvalidDataException(string.Format("Failed to load object of type {0}", typeof(T)));
                        collection.Add(loadedObject);
                    }));
            }

            public void WriteModelCollection<T>(ModelCollection<T> collection, StorageFolder folder) where T : ModelBase
            {
                Contract.Requires(null != collection);
                Contract.Requires(null != folder);

                int index = 0;

                foreach(T model in collection)
                {
                    folder.CreateWriteStreamAndCall(string.Format("{0}.object", index.ToString("D12")),
                        stream => _modelSerializer.WriteObject(stream, model));
                    ++index;
                }
            }

            private void ReadWorkspace(Workspace workspace, StorageFolder workspaceFolder)
            {
                Contract.Requires(null != workspace);
                Contract.Requires(null != workspaceFolder);

                Workspace loadedWorkspace = ReadModelObject<Workspace>(workspaceFolder, ".workspace");
                loadedWorkspace.CopyTo(workspace);
                workspaceFolder.GetFolderAndCall("connections", connectionsFolder => ReadModelCollection(workspace.Connections, connectionsFolder));
                foreach (RemoteConnection rc in workspace.Connections)
                    rc.AttachToWorkspace(workspace);
                //
                // TODO: load credentials from the pasword vault
                //
                workspaceFolder.GetFolderAndCall("credentials", credentialsFolder => ReadModelCollection(workspace.Credentials, credentialsFolder));
            }

            private void WriteWorkspace(Workspace workspace, StorageFolder workspaceFolder)
            {
                Contract.Requires(null != workspace);
                Contract.Requires(null != workspaceFolder);

                workspaceFolder.CreateWriteStreamAndCall(".workspace", stream => _modelSerializer.WriteObject(stream, workspace));
                workspaceFolder.CreateFolderAndCall("connections", connectionsFolder =>
                    WriteModelCollection(workspace.Connections, connectionsFolder));
                //
                // TODO: save credentials in the pasword vault
                //
                workspaceFolder.CreateFolderAndCall("credentials", credentialsFolder =>
                    WriteModelCollection(workspace.Credentials, credentialsFolder));
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

        void IDataStorage.Load(RdDataModel dataModel)
        {
            Contract.Requires(null != dataModel);
            Contract.Assert(null != _rootFolder);

            using(SerializationContext sctx = new SerializationContext(_rootFolder))
            {
                sctx.ReadDataModel(dataModel);
            }
        }

        void IDataStorage.Save(RdDataModel dataModel)
        {
            Contract.Requires(null != dataModel);
            Contract.Assert(null != _rootFolder);

            using (SerializationContext sctx = new SerializationContext(_rootFolder))
            {
                sctx.WriteDataModel(dataModel);
            }
        }
    }
}
