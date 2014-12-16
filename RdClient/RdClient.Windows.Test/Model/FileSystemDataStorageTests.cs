
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

            for (int i = 0; i < 30; ++i)
            {
                saved.LocalWorkspace.Credentials.Add(new Credentials()
                {
                    Username = string.Format("user-{0}", i),
                    Password = string.Format("password-{0}", i),
                    Domain = string.Format("domain-{0}", i)
                });
            }

            IDataStorage
                savingStorage = new FileSystemDataStorage() { RootFolder = "TestFolder" },
                loadingStorage = new FileSystemDataStorage() { RootFolder = "TestFolder" };

            savingStorage.Save(saved);
            loadingStorage.Load(loaded);

            Assert.AreEqual(saved.LocalWorkspace.Id, loaded.LocalWorkspace.Id);
            Assert.AreEqual(saved.LocalWorkspace.Connections.Count, loaded.LocalWorkspace.Connections.Count);
            Assert.AreEqual(saved.LocalWorkspace.Credentials.Count, loaded.LocalWorkspace.Credentials.Count);
            for (int i = 0; i < saved.LocalWorkspace.Connections.Count; ++i)
            {
                Assert.AreEqual(saved.LocalWorkspace.Connections[i].Id, loaded.LocalWorkspace.Connections[i].Id);
                Assert.IsInstanceOfType(loaded.LocalWorkspace.Connections[i], saved.LocalWorkspace.Connections[i].GetType());
                Assert.AreSame(loaded.LocalWorkspace.Connections[i].ParentWorkspace, loaded.LocalWorkspace);
            }
            for (int i = 0; i < saved.LocalWorkspace.Credentials.Count; ++i)
            {
                Assert.AreEqual(saved.LocalWorkspace.Credentials[i].Id, loaded.LocalWorkspace.Credentials[i].Id);
                Assert.IsInstanceOfType(loaded.LocalWorkspace.Credentials[i], saved.LocalWorkspace.Credentials[i].GetType());
                Assert.AreEqual(saved.LocalWorkspace.Credentials[i].Username, loaded.LocalWorkspace.Credentials[i].Username);
                Assert.AreEqual(saved.LocalWorkspace.Credentials[i].Password, loaded.LocalWorkspace.Credentials[i].Password);
                Assert.AreEqual(saved.LocalWorkspace.Credentials[i].Domain, loaded.LocalWorkspace.Credentials[i].Domain);
            }
            Assert.AreSame(loaded, loaded.LocalWorkspace.ParentDataModel);
        }
    }
}
