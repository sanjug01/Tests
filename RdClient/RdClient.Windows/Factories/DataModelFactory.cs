using RdClient.Models;
using RdClient.Shared.Models;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace RdClient.Factories
{
    public class DataModelFactory
    {
        public async Task<IDataModel> CreateDataModel()
        {
            DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            DataContractSerializer serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFolder rootFolder = await roamingFolder.CreateFolderAsync("AppDataStorage", CreationCollisionOption.OpenIfExists);
            AppDataStorage dataStorage = new AppDataStorage() { RootFolder = rootFolder, Serializer = serializer };
            DataModel dataModel = new DataModel() { Storage = dataStorage };
            await dataModel.LoadFromStorage();

            return dataModel;
        }
    }
}
