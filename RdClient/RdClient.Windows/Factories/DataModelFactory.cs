using RdClient.Models;
using RdClient.Shared.Models;
using System.Runtime.Serialization;
using Windows.Storage;

namespace RdClient.Factories
{
    public class DataModelFactory
    {
        public IDataModel CreateDataModel()
        {
            DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            DataContractSerializer serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFolder rootFolder = roamingFolder.CreateFolderAsync("AppDataStorage", CreationCollisionOption.OpenIfExists).GetResults();
            AppDataStorage dataStorage = new AppDataStorage() { RootFolder = rootFolder, Serializer = serializer };
            DataModel dataModel = new DataModel() { Storage = dataStorage };
            dataModel.LoadFromStorage();

            return dataModel;
        }
    }
}
