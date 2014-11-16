using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

using RdClient.Shared.Models;



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

        public async Task<IEnumerable<string>> GetCollectionNames()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            return (await this.RootFolder.GetFoldersAsync()).Select(s => s.Name);
        }

        public async Task<IEnumerable<ModelBase>> LoadCollection(string collectionName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            List<ModelBase> collection = new List<ModelBase>();
            StorageFolder folder = await this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists);
            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    this.Serializer.ReadObject(stream);
                }
            }
            return collection;
        }

        public async Task<bool> DeleteCollection(string collectionName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            bool deleted = false;
            StorageFolder folder = await this.RootFolder.TryGetItemAsync(collectionName) as StorageFolder;
            if (folder != null)
            {
                await folder.DeleteAsync();
                deleted = true;
            }
            return deleted;            
        }

        public async Task SaveItem(string collectionName, ModelBase item)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            StorageFolder folder = await this.RootFolder.CreateFolderAsync(collectionName, CreationCollisionOption.OpenIfExists);
            StorageFile file = await folder.CreateFileAsync(item.Id.ToString(), CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                this.Serializer.WriteObject(stream, item);
            }

        }

        public async Task<bool> DeleteItem(string collectionName, ModelBase item)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Must initialize first");
            }
            bool deleted = false;
            StorageFolder folder = await this.RootFolder.TryGetItemAsync(collectionName) as StorageFolder;
            if (folder != null)
            {
                StorageFile file = await folder.TryGetItemAsync(item.Id.ToString()) as StorageFile;
                if (file != null)
                {
                    await file.DeleteAsync();
                    deleted = true;
                }
            }
            return deleted;
        }
    }
}
