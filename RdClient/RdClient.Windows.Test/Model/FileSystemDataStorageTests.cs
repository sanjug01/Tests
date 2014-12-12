using Windows.Storage;

namespace RdClient.Windows.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Models;
    using RdClient.Shared.Models;
    using System;
    using System.Runtime.Serialization;

    [TestClass]
    public sealed class FileSystemDataStorageTests
    {
        [TestMethod]
        public void NewSerializationContext_WriteReadDesktop_Identical()
        {
            using(FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                Desktop
                    saved = new Desktop() { HostName = "HostName" },
                    loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "desktop.object");
                loaded = ctx.ReadModelObject<Desktop>(folder, "desktop.object");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.HostName, loaded.HostName);
                Assert.AreEqual(saved.Id, loaded.Id);
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadRemoteApplication_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                RemoteApplication
                    saved = new RemoteApplication(),
                    loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "application.object");
                loaded = ctx.ReadModelObject<RemoteApplication>(folder, "application.object");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadLocalWorkspace_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                LocalWorkspace saved = new LocalWorkspace();
                Workspace loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "local.workspace");
                loaded = ctx.ReadModelObject<Workspace>(folder, "local.workspace");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
                Assert.IsInstanceOfType(loaded, saved.GetType());
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadCloudWorkspace_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                CloudWorkspace saved = new CloudWorkspace();
                Workspace loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "cloud.workspace");
                loaded = ctx.ReadModelObject<Workspace>(folder, "cloud.workspace");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
                Assert.IsInstanceOfType(loaded, saved.GetType());
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadOnPremiseWorkspace_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                OnPremiseWorkspace saved = new OnPremiseWorkspace();
                Workspace loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "onpremise.workspace");
                loaded = ctx.ReadModelObject<Workspace>(folder, "onpremise.workspace");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
                Assert.IsInstanceOfType(loaded, saved.GetType());
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadCredentials_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                Credentials saved = new Credentials()
                {
                    Username = "username",
                    Password = "password",
                    Domain = "domain",
                    HaveBeenPersisted = true
                };
                Credentials loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "credentials.object");
                loaded = ctx.ReadModelObject<Credentials>(folder, "credentials.object");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
                Assert.AreEqual(saved.Username, loaded.Username);
                Assert.AreEqual(saved.Password, loaded.Password);
                Assert.AreEqual(saved.Domain, loaded.Domain);
                Assert.AreEqual(saved.HaveBeenPersisted, loaded.HaveBeenPersisted);
            }
        }

        [TestMethod]
        public void NewSerializationContext_WriteReadTrustedCertificate_Identical()
        {
            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                TrustedCertificate saved = new TrustedCertificate()
                {
                    Hash = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                    SerialNumber = new byte[] { 0, 1, 0, 2, 0, 3, 0, 4 }
                };
                TrustedCertificate loaded;
                StorageFolder folder = ctx.CreateRootFolder("TestFolder");

                ctx.WriteModelObject(saved, folder, "certificate.object");
                loaded = ctx.ReadModelObject<TrustedCertificate>(folder, "certificate.object");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Id, loaded.Id);
                Assert.AreEqual(saved.Hash.Length, loaded.Hash.Length);
                for (int i = 0; i < saved.Hash.Length; ++i)
                    Assert.AreEqual(saved.Hash[i], loaded.Hash[i]);
                Assert.AreEqual(saved.SerialNumber.Length, loaded.SerialNumber.Length);
                for (int i = 0; i < saved.SerialNumber.Length; ++i)
                    Assert.AreEqual(saved.SerialNumber[i], loaded.SerialNumber[i]);
            }
        }

        [TestMethod]
        public void WriteReadDesktopCollection_IdenticalCollectionLoaded()
        {
            ModelCollection<Desktop> saved = new ModelCollection<Desktop>();
            ModelCollection<Desktop> loaded = new ModelCollection<Desktop>();

            saved.Add(new Desktop() { HostName = "desktop-a", CredentialId = Guid.NewGuid() });
            saved.Add(new Desktop() { HostName = "desktop-b", CredentialId = Guid.NewGuid() });
            saved.Add(new Desktop() { HostName = "desktop-0", CredentialId = Guid.NewGuid() });
            saved.Add(new Desktop() { HostName = "desktop-2", CredentialId = Guid.NewGuid() });

            using (FileSystemDataStorage.SerializationContext ctx = new FileSystemDataStorage.SerializationContext("TestFolder"))
            {
                StorageFolder folder = ctx.CreateRootFolder("Desktops");

                ctx.WriteModelCollection(saved, folder, "Desktops");
                ctx.ReadModelCollection(loaded, folder, "Desktops");

                folder.DeleteAsync().AsTask().Wait();

                Assert.AreEqual(saved.Count, loaded.Count);

                for( int i = 0; i < saved.Count; ++i )
                {
                    Assert.AreEqual(saved[i].HostName, loaded[i].HostName);
                    Assert.AreEqual(saved[i].Id, loaded[i].Id);
                }
            }
        }
    }
}
