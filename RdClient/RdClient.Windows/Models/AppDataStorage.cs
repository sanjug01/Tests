using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace RdClient.Models
{
    public class AppDataStorage : IDataStorage
    {
        private DataContractSerializer _serializer = null;
        private StorageFolder _rootFolder = null;

        public DataContractSerializer Serializer
        {
            get
            {
                return _serializer;
            }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Cannot set once initialized");
                }
                else
                {
                    _serializer = value;
                }
            }
        }

        public StorageFolder RootFolder
        {
            get
            {
                return _rootFolder;
            }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Cannot set once initialized");
                }
                else
                {
                    _rootFolder = value;
                }
            }
        }

        public bool Initialized
        {
            get
            {
                return (this.Serializer != null && this.RootFolder != null);
            }
        }

        public IEnumerable<string> GetCollectionNames()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }

            Task<IReadOnlyList<StorageFolder>> folderTask = this.RootFolder.GetFoldersAsync().AsTask<IReadOnlyList<StorageFolder>>();
            folderTask.Wait();

            IReadOnlyList<StorageFolder> folderList = folderTask.Result;

            return folderList.Select(s => s.Name);
        }

        public IEnumerable<ModelBase> LoadCollection(string collectionName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            List<ModelBase> collection = new List<ModelBase>();
            
            Task<StorageFolder> folderTask = this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists).AsTask<StorageFolder>();
            folderTask.Wait();

            Task<IReadOnlyList<StorageFile>> filesTask = folderTask.Result.GetFilesAsync().AsTask<IReadOnlyList<StorageFile>>();
            filesTask.Wait();

            foreach (StorageFile file in filesTask.Result)
            {
                Task<Stream> streamTask = file.OpenStreamForReadAsync();
                streamTask.Wait();

                using (Stream stream = streamTask.Result)
                {
                    ModelBase item = this.Serializer.ReadObject(stream) as ModelBase;
                    if (item == null)
                    {
                        throw new SerializationException("Failed to deserialize " + file.Path);                        
                    }
                    else
                    {
                        collection.Add(item);
                    }
                }
            }
            return collection;
        }

        public void DeleteCollection(string collectionName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }

            Task<IStorageItem> folderTask = this.RootFolder.TryGetItemAsync(collectionName).AsTask<IStorageItem>();
            folderTask.Wait();

            StorageFolder folder = folderTask.Result as StorageFolder;
 
            if(folder == null)
            {
                throw new SerializationException("Failed to delete folder: " + collectionName);
            }

            Task deleteTask = folder.DeleteAsync().AsTask();
            deleteTask.Wait();
        }

        public void SaveItem(string collectionName, ModelBase item)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            Task<StorageFolder> folderTask = this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists).AsTask<StorageFolder>();
            folderTask.Wait();

            Task<StorageFile> fileTask = folderTask.Result.CreateFileAsync(item.Id.ToString(), CreationCollisionOption.ReplaceExisting).AsTask<StorageFile>();
            fileTask.Wait();

            Task<Stream> streamTask = fileTask.Result.OpenStreamForWriteAsync();
            streamTask.Wait();

            using (Stream stream = streamTask.Result)
            {
                this.Serializer.WriteObject(stream, item);                
            }
        }

        public void DeleteItem(string collectionName, ModelBase item)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }

            Task<IStorageItem> folderTask = this.RootFolder.TryGetItemAsync(collectionName).AsTask<IStorageItem>();
            folderTask.Wait();

            StorageFolder folder = folderTask.Result as StorageFolder;
            if (folder == null)
            {
                throw new SerializationException("Couldn't find folder: " + collectionName + " for item " + item);
            }

            if (folder != null)
            {
                Task<IStorageItem> fileTask = folder.TryGetItemAsync(item.Id.ToString()).AsTask<IStorageItem>();
                fileTask.Wait();

                StorageFile file = fileTask.Result as StorageFile;
                if (file == null)
                {
                    throw new SerializationException("Couldn't find file: " + item);
                }

                Task deleteTask = file.DeleteAsync().AsTask();
                deleteTask.Wait();                
            }
        }
    }
}
