
namespace RdClient.Windows.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Models;
    using RdClient.Shared.Models;

    [TestClass]
    public sealed class FileSystemDataStorageTests
    {
        [TestMethod]
        public void WriteReadDataModel_IdenticalDataModelLoaded()
        {
            RdDataModel saved = new RdDataModel(), loaded = new RdDataModel();

            for (int i = 0; i < 25; ++i)
            {
                Desktop desktop = new Desktop(saved.LocalWorkspace) { HostName = string.Format("hostname-{0}", i) };
                RemoteApplication application = new RemoteApplication(saved.LocalWorkspace);

                saved.LocalWorkspace.Connections.Add(desktop);
                saved.LocalWorkspace.Connections.Add(application);
            }

            IDataStorage
                savingStorage = new FileSystemDataStorage() { RootFolder = "TestFolder" },
                loadingStorage = new FileSystemDataStorage() { RootFolder = "TestFolder" };

            savingStorage.Save(saved);
            loadingStorage.Load(loaded);

            Assert.AreEqual(saved.LocalWorkspace.Id, loaded.LocalWorkspace.Id);
            Assert.AreEqual(saved.LocalWorkspace.Connections.Count, loaded.LocalWorkspace.Connections.Count);
            for (int i = 0; i < saved.LocalWorkspace.Connections.Count; ++i)
            {
                Assert.AreEqual(saved.LocalWorkspace.Connections[i].Id, loaded.LocalWorkspace.Connections[i].Id);
                Assert.IsInstanceOfType(loaded.LocalWorkspace.Connections[i], saved.LocalWorkspace.Connections[i].GetType());
                Assert.AreSame(loaded.LocalWorkspace.Connections[i].ParentWorkspace, loaded.LocalWorkspace);
            }
            Assert.AreSame(loaded, loaded.LocalWorkspace.ParentDataModel);
        }
    }
}
