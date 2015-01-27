namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;

    [TestClass]
    public sealed class WorkspaceModelTests
    {
        [TestMethod]
        public void LocalWorkspace_LoadFromEmptyFolder_NewCleanWorkspace()
        {
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            Assert.IsNotNull(workspace.WorkspaceData);
            Assert.IsNotNull(workspace.Credentials.Models);
            Assert.IsNotNull(workspace.Connections.Models);
        }

        [TestMethod]
        public void LocalWorkspace_LoadFromEmptyFolderChangeSave_Saved()
        {
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();
            int folderCount = 0;

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            Assert.IsFalse(workspace.Save.CanExecute(null));
            workspace.Credentials.AddNewModel(new CredentialsModel() { Username = "User", Password = "Password", Domain = "Domain" });
            Assert.IsTrue(workspace.Save.CanExecute(null));
            workspace.Save.Execute(null);
            Assert.IsFalse(workspace.Save.CanExecute(null));

            foreach(string folderName in folder.GetFolders())
            {
                Assert.IsTrue(string.Equals("credentials", folderName) || string.Equals("connections", folderName));
                ++folderCount;
            }

            Assert.AreEqual(2, folderCount);
        }
    }
}
