using Windows.Storage;

namespace RdClient.Windows.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [TestClass]
    public sealed class WorkspaceModelSerializationTests
    {
        static readonly string RootFolderName = "SerializationTestRoot";

        RdClient.Shared.Data.IStorageFolder _root;

        [TestInitialize]
        public void SetUpTest()
        {
            _root = new RdClient.Data.ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            //
            // Delete the test root folder;
            //
            try
            {
                ApplicationData.Current.LocalFolder.GetFolderAsync(RootFolderName).AsTask<StorageFolder>()
                    .Result.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().Wait();
            }
            catch (Exception ex)
            {
                //
                // The folder is deleted after each test (just in case) but a failure is reported only
                // if the folder was expected to be there.
                //
                Debug.WriteLine("TearDownTest|Failed to delete the serializarion test root folder|{0}", ex.Message);
            }
        }

        [TestMethod]
        public void LocalWorkspace_SaveLoad_SameWorkspaceLoaded()
        {
            RdClient.Shared.Data.IModelSerializer serializer = new RdClient.Shared.Data.SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> savedWorkspace = new WorkspaceModel<LocalWorkspaceModel>(_root, serializer);
            IPersistentObject po = savedWorkspace;

            savedWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username="user", Password="password" });
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);

            WorkspaceModel<LocalWorkspaceModel> loadedWorkspace = new WorkspaceModel<LocalWorkspaceModel>(_root, serializer);

            Assert.IsNotNull(loadedWorkspace);
            Assert.IsNotNull(loadedWorkspace.Connections);
            Assert.IsNotNull(loadedWorkspace.Connections.Models);

            Assert.AreEqual(savedWorkspace.WorkspaceData, loadedWorkspace.WorkspaceData);
            Assert.IsNotNull(loadedWorkspace.Credentials);
            Assert.IsNotNull(loadedWorkspace.Credentials.Models);
            Assert.AreEqual(savedWorkspace.Credentials.Models.Count, loadedWorkspace.Credentials.Models.Count);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Id, loadedWorkspace.Credentials.Models[0].Id);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Username, loadedWorkspace.Credentials.Models[0].Model.Username);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Password, loadedWorkspace.Credentials.Models[0].Model.Password);
            Assert.IsFalse(po.Save.CanExecute(null));
        }

        [TestMethod]
        public void OnPremiseWorkspace_SaveLoad_SameWorkspaceLoaded()
        {
            RdClient.Shared.Data.IModelSerializer serializer = new RdClient.Shared.Data.SerializableModelSerializer();

            WorkspaceModel<OnPremiseWorkspaceModel> savedWorkspace = new WorkspaceModel<OnPremiseWorkspaceModel>(_root, serializer);
            IPersistentObject po = savedWorkspace;

            savedWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "user", Password = "password" });
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);

            WorkspaceModel<OnPremiseWorkspaceModel> loadedWorkspace = new WorkspaceModel<OnPremiseWorkspaceModel>(_root, serializer);
            IPersistentObject poLoaded = loadedWorkspace;

            Assert.IsNotNull(loadedWorkspace);
            Assert.IsNotNull(loadedWorkspace.Connections);
            Assert.IsNotNull(loadedWorkspace.Connections.Models);

            Assert.IsNotNull(loadedWorkspace.Credentials);
            Assert.IsNotNull(loadedWorkspace.Credentials.Models);
            Assert.AreEqual(savedWorkspace.Credentials.Models.Count, loadedWorkspace.Credentials.Models.Count);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Id, loadedWorkspace.Credentials.Models[0].Id);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Username, loadedWorkspace.Credentials.Models[0].Model.Username);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Password, loadedWorkspace.Credentials.Models[0].Model.Password);
            Assert.IsFalse(poLoaded.Save.CanExecute(null));
        }

        [TestMethod]
        public void CloudWorkspace_SaveLoad_SameWorkspaceLoaded()
        {
            RdClient.Shared.Data.IModelSerializer serializer = new RdClient.Shared.Data.SerializableModelSerializer();

            WorkspaceModel<CloudWorkspaceModel> savedWorkspace = new WorkspaceModel<CloudWorkspaceModel>(_root, serializer);
            IPersistentObject po = savedWorkspace;

            savedWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "user", Password = "password" });
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);

            WorkspaceModel<CloudWorkspaceModel> loadedWorkspace = new WorkspaceModel<CloudWorkspaceModel>(_root, serializer);
            IPersistentObject poLoaded = loadedWorkspace;

            Assert.IsNotNull(loadedWorkspace);
            Assert.IsNotNull(loadedWorkspace.Connections);
            Assert.IsNotNull(loadedWorkspace.Connections.Models);

            Assert.IsNotNull(loadedWorkspace.Credentials);
            Assert.IsNotNull(loadedWorkspace.Credentials.Models);
            Assert.AreEqual(savedWorkspace.Credentials.Models.Count, loadedWorkspace.Credentials.Models.Count);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Id, loadedWorkspace.Credentials.Models[0].Id);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Username, loadedWorkspace.Credentials.Models[0].Model.Username);
            Assert.AreEqual(savedWorkspace.Credentials.Models[0].Model.Password, loadedWorkspace.Credentials.Models[0].Model.Password);
            Assert.IsFalse(poLoaded.Save.CanExecute(null));
        }
    }
}
