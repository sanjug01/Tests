﻿using RdClient.Models;
using RdClient.Shared.Models;
using System.Runtime.Serialization;
using Windows.Storage;

namespace RdClient.Factories
{
    public class DataModelFactory
    {
        public RdDataModel CreateDataModel()
        {
            //DataContractSerializerSettings serializerSettings = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            //DataContractSerializer serializer = new DataContractSerializer(typeof(ModelBase), serializerSettings);
            //StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            //StorageFolder rootFolder = roamingFolder.CreateFolderAsync("AppDataStorage", CreationCollisionOption.OpenIfExists).GetResults();
            FileSystemDataStorage dataStorage = new FileSystemDataStorage() { RootFolder = "AppDataStorage" };
            RdDataModel dataModel = new RdDataModel() { Storage = dataStorage };
            dataModel.Load();

            return dataModel;
        }
    }
}
