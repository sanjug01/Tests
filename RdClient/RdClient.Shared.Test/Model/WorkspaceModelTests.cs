namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    [TestClass]
    public sealed class WorkspaceModelTests
    {
        [DataContract]
        private sealed class TestWorkspaceData : SerializableModel
        {
            [DataMember(Name = "Property")]
            private int _property;

            public int Property
            {
                get { return _property; }
                set { this.SetProperty(ref _property, value); }
            }
        }

        private sealed class TestModelSerializer : IModelSerializer
        {
            private readonly DataContractSerializer _serializer = new DataContractSerializer(typeof(TestWorkspaceData));

            TModel IModelSerializer.ReadModel<TModel>(Stream stream)
            {
                return _serializer.ReadObject(stream) as TModel;
            }

            void IModelSerializer.WriteModel<TModel>(TModel model, Stream stream)
            {
                _serializer.WriteObject(stream, model);
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
            workspace.Credentials.AddNewModel(new CredentialsModel() { Username = "User", Password = "Password" });
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);
            Assert.IsFalse(po.Save.CanExecute(null));

            List<string> folders = folder.GetFolders().ToList();
            List<string> files = folder.GetFiles().ToList();

            Assert.AreEqual(2, folders.Count);
            Assert.IsTrue(folders.Contains("credentials"));
            Assert.IsTrue(folders.Contains("connections"));
            Assert.AreEqual(0, files.Count);
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
        public void WorkspaceModel_ChangeWorkspaceDataSave_DataSaved()
        {
            IStorageFolder folder = new MemoryStorageFolder();
            IModelSerializer serializer = new TestModelSerializer();

            WorkspaceModel<TestWorkspaceData> workspace = new WorkspaceModel<TestWorkspaceData>(folder, serializer);
            IPersistentObject po = workspace;
            Assert.IsFalse(po.Save.CanExecute(null));

            workspace.WorkspaceData.Property += 1;
            Assert.IsTrue(po.Save.CanExecute(null));
            po.Save.Execute(null);

            Assert.IsFalse(po.Save.CanExecute(null));
            IList<string> files = folder.GetFiles().ToList();
            Assert.AreEqual(1, files.Count);
            Assert.IsTrue(files.Contains(".workspace"));
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
            Assert.AreEqual(2, changes.Count);
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
            Assert.AreEqual(2, changes.Count);
            foreach (ICommand c in changes)
                Assert.AreSame(po.Save, c);
        }
    }
}
