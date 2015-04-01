﻿namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Mock;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    
    [TestClass]
    public sealed class ApplicationDataModelTests
    {
        [TestMethod]
        public void NewApplicationDataModel_CannotBeSaved()
        {
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;

            Assert.IsNotNull(adm.RootFolder);
            Assert.IsNotNull(adm.ModelSerializer);
            Assert.IsNotNull(adm.LocalWorkspace);
            Assert.IsNotNull(adm.CertificateTrust);
            Assert.IsFalse(po.Save.CanExecute(null));
        }

        [TestMethod]
        public void NewApplicationDataModel_AddLocalDesktop_CanBeSaved()
        {
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;

            adm.LocalWorkspace.Connections.AddNewModel(new DesktopModel());

            Assert.IsTrue(po.Save.CanExecute(null));
        }

        [TestMethod]
        public void NewApplicationDataModel_AddLocalCredentials_CanBeSaved()
        {
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;

            adm.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel());

            Assert.IsTrue(po.Save.CanExecute(null));
        }

        [TestMethod]
        public void NewApplicationDataModel_AddLocalDesktopSave_Saved()
        {
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;

            adm.LocalWorkspace.Connections.AddNewModel(new DesktopModel());
            po.Save.Execute(null);

            Assert.IsFalse(po.Save.CanExecute(null));
        }

        [TestMethod]
        public void NewApplicationDataModel_AddLocalDesktopSaveChangeDesktop_CanBeSaved()
        {
            IList<ICommand> changes = new List<ICommand>();
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;

            adm.LocalWorkspace.Connections.AddNewModel(new DesktopModel());
            po.Save.Execute(null);
            DesktopModel dtm = adm.LocalWorkspace.Connections.Models[0].Model as DesktopModel;
            Assert.IsNotNull(dtm);
            Assert.IsFalse(po.Save.CanExecute(null));
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);

            dtm.HostName = "NewHostName";

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void NewApplicationDataModel_AddLocalCredentialsSaveChangeCredentials_CanBeSaved()
        {
            IList<ICommand> changes = new List<ICommand>();
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                ModelSerializer = new SerializableModelSerializer(),
                RootFolder = new MemoryStorageFolder()
            };
            IPersistentObject po = adm;

            adm.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel());
            po.Save.Execute(null);
            CredentialsModel cred = adm.LocalWorkspace.Credentials.Models[0].Model;
            Assert.IsNotNull(cred);
            Assert.IsFalse(po.Save.CanExecute(null));
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);

            cred.Username = "DifferentUser";

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void NewApplicationDataModel_TrustCertificate_CanBeSaved()
        {
            IList<ICommand> changes = new List<ICommand>();
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);

            adm.CertificateTrust.TrustCertificate(new TestRdpCertificate());

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void NewApplicationDataModel_TrustCertificateSave_CannotBeSaved()
        {
            IList<ICommand> changes = new List<ICommand>();
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);

            adm.CertificateTrust.TrustCertificate(new TestRdpCertificate());
            po.Save.Execute(null);

            IList<string> files = adm.RootFolder.GetFiles().ToList();
            Assert.IsFalse(po.Save.CanExecute(null));
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
            Assert.AreSame(po.Save, changes[1]);
            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("CertificateTrust.model", files[0]);
        }

        [TestMethod]
        public void ApplicationDataModel_RemoveCredentials_DesktopsUpdated()
        {
            ApplicationDataModel adm = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            IPersistentObject po = adm;
            Guid credentialsId = adm.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel());
            for (int i = 0; i < 100; ++i )
            {
                DesktopModel desktop = new DesktopModel();
                adm.LocalWorkspace.Connections.AddNewModel(desktop);
            }
            po.Save.Execute(null);

            adm.LocalWorkspace.Credentials.RemoveModel(credentialsId);

            int desktops = 0;

            foreach(IModelContainer<RemoteConnectionModel> container in adm.LocalWorkspace.Connections.Models)
            {
                if(container.Model is DesktopModel)
                {
                    Assert.AreEqual(Guid.Empty, ((DesktopModel)container.Model).CredentialsId);
                    ++desktops;
                }
            }

            Assert.AreNotEqual(0, desktops);
        }
    }
}
