﻿namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    [TestClass]
    public sealed class WorkspaceModelTests
    {
        private class TestWorkspaceData : MutableObject
        {
            private int _property;

            public int Property
            {
                get { return _property; }
                set { this.SetProperty(ref _property, value); }
            }
        }

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
            IPersistentObject po = workspace;

            Assert.IsFalse(po.Save.CanExecute(null));
            workspace.Credentials.AddNewModel(new CredentialsModel() { Username = "User", Password = "Password", Domain = "Domain" });
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);
            Assert.IsFalse(po.Save.CanExecute(null));

            List<string> folders = folder.GetFolders().ToList();
            List<string> files = folder.GetFiles().ToList();

            Assert.AreEqual(2, folders.Count);
            Assert.IsTrue(folders.Contains("credentials"));
            Assert.IsTrue(folders.Contains("connections"));
            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(".workspace", files[0]);
        }

        [TestMethod]
        public void WorkspaceModel_ChangeWorkspaceData_CanSave()
        {
            IList<ICommand> changes = new List<ICommand>();
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<TestWorkspaceData> workspace = new WorkspaceModel<TestWorkspaceData>(folder, serializer);
            IPersistentObject po = workspace;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.WorkspaceData.Property += 1;

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void WorkspaceModel_AddDesktop_CanSave()
        {
            IList<ICommand> changes = new List<ICommand>();
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<TestWorkspaceData> workspace = new WorkspaceModel<TestWorkspaceData>(folder, serializer);
            IPersistentObject po = workspace;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.Connections.AddNewModel(new DesktopModel());

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void WorkspaceModel_AddCredentials_CanSave()
        {
            IList<ICommand> changes = new List<ICommand>();
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<TestWorkspaceData> workspace = new WorkspaceModel<TestWorkspaceData>(folder, serializer);
            IPersistentObject po = workspace;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.Credentials.AddNewModel(new CredentialsModel());

            Assert.IsTrue(po.Save.CanExecute(null));
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(po.Save, changes[0]);
        }

        [TestMethod]
        public void LocalWorkspaceModel_AddDesktopSave_CannotSave()
        {
            IList<ICommand> changes = new List<ICommand>();
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            IPersistentObject po = workspace;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.Connections.AddNewModel(new DesktopModel());
            po.Save.Execute(null);

            Assert.IsFalse(po.Save.CanExecute(null));
            Assert.AreEqual(3, changes.Count);
            foreach(ICommand c in changes)
                Assert.AreSame(po.Save, c);
        }

        [TestMethod]
        public void LocalWorkspaceModel_AddCredentialsSave_CannotSave()
        {
            IList<ICommand> changes = new List<ICommand>();
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new SerializableModelSerializer();

            WorkspaceModel<LocalWorkspaceModel> workspace = new WorkspaceModel<LocalWorkspaceModel>(folder, serializer);
            IPersistentObject po = workspace;
            po.Save.CanExecuteChanged += (sender, e) => changes.Add((ICommand)sender);
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.Credentials.AddNewModel(new CredentialsModel());
            po.Save.Execute(null);

            Assert.IsFalse(po.Save.CanExecute(null));
            Assert.AreEqual(3, changes.Count);
            foreach (ICommand c in changes)
                Assert.AreSame(po.Save, c);
        }
    }
}
