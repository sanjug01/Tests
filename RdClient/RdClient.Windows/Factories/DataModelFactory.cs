using RdClient.Models;
using RdClient.Shared.LifeTimeManagement;
using RdClient.Shared.Models;

namespace RdClient.Factories
{
    public class DataModelFactory
    {
        public RdDataModel CreateDataModel(ILifeTimeManager lifeTimeManager)
        {
            //DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            //DataContractSerializer serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            //StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            //StorageFolder rootFolder = roamingFolder.CreateFolderAsync("AppDataStorage", CreationCollisionOption.OpenIfExists).GetResults();
            FileSystemDataStorage dataStorage = new FileSystemDataStorage() { RootFolder = "AppDataStorage" };

            RdDataModel dataModel = new RdDataModel()
            {
                Storage = dataStorage,
                LifeTimeManager = lifeTimeManager
            };

            return dataModel;
        }
    }
}
