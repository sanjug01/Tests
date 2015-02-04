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

            return this.RootFolder.GetFoldersAsync()
                .AsTask<IReadOnlyList<StorageFolder>>()
                .Result
                .Select(s => s.Name);
        }

        public IEnumerable<ModelBase> LoadCollection(string collectionName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            List<ModelBase> collection = new List<ModelBase>();
            
            Task<StorageFolder> folderTask = this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists).AsTask<StorageFolder>();
            Task<IReadOnlyList<StorageFile>> filesTask = folderTask.Result.GetFilesAsync().AsTask<IReadOnlyList<StorageFile>>();

            foreach (StorageFile file in filesTask.Result)
            {
                using (Stream stream = file.OpenStreamForReadAsync().Result)
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

            StorageFolder folder = this.RootFolder.TryGetItemAsync(collectionName).AsTask<IStorageItem>().Result as StorageFolder;
 
            if(folder == null)
            {
                throw new SerializationException("Failed to delete folder: " + collectionName);
            }

            folder.DeleteAsync().AsTask().Wait();
        }

        public void SaveItem(string collectionName, ModelBase item)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }

            Task<StorageFolder> folderTask = this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists)
                .AsTask<StorageFolder>();

            Task<StorageFile> fileTask = folderTask.Result.CreateFileAsync(item.Id.ToString(), CreationCollisionOption.ReplaceExisting)
                .AsTask<StorageFile>();

            using (Stream stream = fileTask.Result.OpenStreamForWriteAsync().Result)
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

            StorageFolder folder = this.RootFolder.TryGetItemAsync(collectionName).AsTask<IStorageItem>().Result as StorageFolder;

            if (folder == null)
            {
                throw new SerializationException("Couldn't find folder: " + collectionName + " for item " + item);
            }

            if (folder != null)
            {
                StorageFile file = folder.TryGetItemAsync(item.Id.ToString()).AsTask<IStorageItem>().Result as StorageFile;

                if (file == null)
                {
                    throw new SerializationException("Couldn't find file: " + item);
                }

                file.DeleteAsync().AsTask().Wait();
            }
        }
    }
}
