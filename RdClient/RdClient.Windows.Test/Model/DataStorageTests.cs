using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using RdClient.Shared.Models;
using RdClient.Models;

namespace RdClient.Windows.Test.Model
{
    [TestClass]
    public class DataStorageTests : IDataStorageTests
    {
#if false
        private StorageFolder _storageFolder;

        public override IDataStorage GetDataStorage()
        {
            AppDataStorage dataStorage = new AppDataStorage();
            dataStorage.RootFolder = _storageFolder;
            DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            dataStorage.Serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            return dataStorage;
        }

        public override void DataStorageSetup()
        {
            StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
            Task<StorageFolder> folderTask = tempFolder.CreateFolderAsync("AppDataStorageTests", CreationCollisionOption.ReplaceExisting).AsTask<StorageFolder>();

            _storageFolder = folderTask.Result;             
        }

        public override void DataStorageCleanup()
        {
            _storageFolder.DeleteAsync().AsTask().Wait();
        }
#endif
    }
}
