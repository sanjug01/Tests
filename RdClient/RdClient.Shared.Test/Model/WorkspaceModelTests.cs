namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public sealed class WorkspaceModelTests
    {
        [TestMethod]
        public void LocalWorkspaceModel_BumpUpCodeCoverage()
        {
            LocalWorkspaceModel
                ws1 = new LocalWorkspaceModel(),
                ws2 = new LocalWorkspaceModel();

            //
            // Bump up code coverage
            //
            ws1.GetHashCode();
            ws2.GetHashCode();

            Assert.AreEqual(ws1, ws2);
            Assert.IsTrue(ws1.Equals(ws2));
            Assert.IsTrue(((IEquatable<LocalWorkspaceModel>)ws1).Equals(ws2));
        }

        [TestMethod]
        public void LocalWorkspace_LoadFromEmptyFolder_NewCleanWorkspace()
        {
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            Assert.IsNotNull(workspace.WorkspaceData);
            Assert.IsNotNull(workspace.Credentials.Models);
            Assert.AreEqual(0, workspace.Credentials.Models.Count);
            Assert.IsNotNull(workspace.Connections.Models);
            Assert.AreEqual(0, workspace.Connections.Models.Count);
        }

        [TestMethod]
        public void LocalWorkspace_LoadFromEmptyFolderChangeSave_Saved()
        {
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            Assert.IsFalse(workspace.Save.CanExecute(null));
            workspace.Credentials.AddNewModel(new CredentialsModel() { Username = "User", Password = "Password", Domain = "Domain" });
            Assert.IsTrue(workspace.Save.CanExecute(null));
            workspace.Save.Execute(null);
            Assert.IsFalse(workspace.Save.CanExecute(null));

            List<string> folders = folder.GetFolders().ToList();
            List<string> files = folder.GetFiles().ToList();

            Assert.AreEqual(2, folders.Count);
            Assert.IsTrue(folders.Contains("credentials"));
            Assert.IsTrue(folders.Contains("connections"));
            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(".workspace", files[0]);
        }
    }
}
