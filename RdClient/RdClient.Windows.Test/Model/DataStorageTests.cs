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
        private StorageFolder _storageFolder;

        public override IDataStorage GetDataStorage()
        {
            AppDataStorage dataStorage = new AppDataStorage();
            dataStorage.RootFolder = _storageFolder;
            DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            dataStorage.Serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            return dataStorage;
        }

        public override async Task DataStorageSetup()
        {
            StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
            _storageFolder = await tempFolder.CreateFolderAsync("AppDataStorageTests", CreationCollisionOption.ReplaceExisting);             
        }

        public override async Task DataStorageCleanup()
        {
            await _storageFolder.DeleteAsync();
        }
    }
}
