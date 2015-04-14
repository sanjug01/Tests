namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.Test.UAP;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [TestClass]
    public sealed class PrimaryModelCollectionTests
    {
        IStorageFolder _emptyFolder;
        IModelSerializer _serializer;

        [TestInitialize]
        public void SetUpTest()
        {
            _emptyFolder = new MemoryStorageFolder();
            _serializer = new TestModelSerializer();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _emptyFolder = null;
            _serializer = null;
        }

        [TestMethod]
        public void NewPrimaryModelCollection_EmptyModels()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            Assert.AreEqual(0, collection.Models.Count);
        }

        [TestMethod]
        public void PrimaryModelCollection_AddNewModel_NewModelAdded()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);
            Guid id = collection.AddNewModel(model);

            Assert.AreEqual(1, collection.Models.Count);

            foreach(IModelContainer<TestModel> container in collection.Models)
            {
                Assert.AreEqual(PersistentStatus.New, container.Status);
                Assert.AreSame(model, container.Model);
                Assert.AreEqual(id, container.Id);
            }
        }

        [TestMethod]
        public void PrimaryModelCollection_AddNewModelGetModel_AddedModelReturned()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);
            Guid id = collection.AddNewModel(model);
            for (int i = 0; i < 100; ++i)
                collection.AddNewModel(new TestModel(i + 100));
            TestModel foundModel = collection.GetModel(id);

            Assert.AreSame(model, foundModel);
        }

        [TestMethod]
        public void PrimaryModelCollection_AddModelsGetModelWithBadId_Throws()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<KeyNotFoundException>(() =>
            {
                IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
                TestModel model = new TestModel(1);
                for (int i = 0; i < 100; ++i)
                    collection.AddNewModel(new TestModel(i));
                TestModel foundModel = collection.GetModel(Guid.NewGuid());
            }));
        }

        [TestMethod]
        public void PrimaryModelCollection_AddRemoveModel_EmptyCollection()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);
            Guid id = collection.AddNewModel(model);
            TestModel removedModel = collection.RemoveModel(id);

            Assert.AreEqual(0, collection.Models.Count);
            Assert.AreSame(model, removedModel);
        }

        [TestMethod]
        public void PrimaryModelCollection_RemoveNonexistingModel_ExceptionThrown()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<KeyNotFoundException>(() =>
            {
                IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
                TestModel model = new TestModel(1);
                collection.AddNewModel(model);
                collection.RemoveModel(Guid.NewGuid());
            }));
        }

        [TestMethod]
        public void PrimaryModelCollection_AddAddRemoveModel_CorrectModelRemoved()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);

            collection.AddNewModel(new TestModel(1));

            TestModel model = new TestModel(2);
            Guid id = collection.AddNewModel(model);
            TestModel removedModel = collection.RemoveModel(id);

            Assert.AreEqual(1, collection.Models.Count);
            Assert.AreSame(model, removedModel);

            foreach(IModelContainer<TestModel> container in collection.Models)
            {
                Assert.AreNotSame(model, container.Model);
                Assert.AreNotEqual(id, container.Id);
            }
        }

        [TestMethod]
        public void LoadPrimaryModelCollection_1TestModel_CleanModelLoaded()
        {
            //
            // Write one TestModel object to the root directory
            //
            TestModel savedModel = new TestModel(10);
            Guid savedId = Guid.NewGuid();
            _serializer.WriteModel(savedModel, _emptyFolder.CreateFile(string.Format("{0}.model", savedId)));
            //
            // Load from the root directory and verify that the same object has been loaded
            //
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            Assert.AreEqual(1, collection.Models.Count);
            Assert.AreEqual(savedModel.Property, collection.Models[0].Model.Property);
            Assert.AreEqual(savedId, collection.Models[0].Id);
            Assert.AreEqual(PersistentStatus.Clean, collection.Models[0].Status);
        }

        [TestMethod]
        public void LoadPrimaryModelCollection_1TestModelSerializerFails_EmptyCollection()
        {
            //
            // Write one TestModel object to the root directory
            //
            TestModel savedModel = new TestModel(10);
            Guid savedId = Guid.NewGuid();
            _emptyFolder.CreateFile("dummy.model");
            _emptyFolder.CreateFile("just_dummy");
            _serializer.WriteModel(savedModel, _emptyFolder.CreateFile(string.Format("{0}.model", savedId)));
            //
            // Load from the root directory and verify that the same object has been loaded
            //
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder,
                new FailingModelSerializer());
            Assert.AreEqual(0, collection.Models.Count);
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddModelSave_SavedInStorage()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            Guid modelId = collection.AddNewModel(new TestModel(1));
            int filesNumber = 0;

            collection.Save.Execute(null);

            foreach(IModelContainer<TestModel> container in collection.Models)
            {
                Assert.AreEqual(PersistentStatus.Clean, container.Status);
                Assert.AreEqual(PersistentStatus.Clean, container.Model.Status);
            }

            foreach(string fileName in _emptyFolder.GetFiles())
            {
                ++filesNumber;
                Assert.AreEqual(string.Format("{0}.model", modelId), fileName);
            }

            foreach(string folderName in _emptyFolder.GetFolders())
            {
                Assert.Fail("Unexpected folder \"{0}\"", folderName);
            }

            Assert.AreEqual(1, filesNumber);

            foreach(IModelContainer<TestModel> container in collection.Models)
            {
                Assert.AreEqual(PersistentStatus.Clean, container.Status);
            }
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddModelSaveRemoveSave_StorageIsEmpty()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            Guid modelId = collection.AddNewModel(new TestModel(1));

            collection.Save.Execute(null);
            collection.Models[0].Model.Property += 1;
            collection.RemoveModel(modelId);
            collection.Save.Execute(null);

            foreach (string fileName in _emptyFolder.GetFiles())
            {
                Assert.Fail("Unexpected file \"{0}\"", fileName);
            }

            foreach (string folderName in _emptyFolder.GetFolders())
            {
                Assert.Fail("Unexpected folder \"{0}\"", folderName);
            }
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddModel_SaveCanExecuteChanged()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);

            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);
            collection.AddNewModel(new TestModel(10));
            Assert.AreEqual(1, reportedSenders.Count);
            Assert.AreSame(collection.Save, reportedSenders[0]);
            Assert.IsTrue(collection.Save.CanExecute(null));
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddModelSave_SaveCanExecuteChanged()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);

            Assert.IsFalse(collection.Save.CanExecute(null));

            Guid modelId = collection.AddNewModel(new TestModel(10));
            collection.Save.Execute(null);
            Assert.IsFalse(collection.Save.CanExecute(null));
            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);
            collection.RemoveModel(modelId);

            Assert.AreEqual(1, reportedSenders.Count);
            Assert.AreSame(collection.Save, reportedSenders[0]);
            Assert.IsTrue(collection.Save.CanExecute(null));
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddRemoveModel_CannotSave()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);

            Assert.IsFalse(collection.Save.CanExecute(null));

            Guid modelId = collection.AddNewModel(new TestModel(10));
            collection.RemoveModel(modelId);

            Assert.IsFalse(collection.Save.CanExecute(null));
            Assert.AreEqual(2, reportedSenders.Count);
            Assert.AreSame(collection.Save, reportedSenders[0]);
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddSaveChange_CanSave()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);

            Guid modelId = collection.AddNewModel(model);
            collection.Save.Execute(null);
            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);

            model.Property += 1;
            Assert.AreEqual(1, reportedSenders.Count);
            Assert.IsTrue(collection.Save.CanExecute(null));
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddAddSaveRemoveChange_1CanSaveChangeReported()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);

            collection.AddNewModel(model);
            Guid modelId = collection.AddNewModel(new TestModel(2));
            collection.Save.Execute(null);
            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);

            collection.RemoveModel(modelId);
            model.Property += 1;
            Assert.AreEqual(1, reportedSenders.Count);
            Assert.IsTrue(collection.Save.CanExecute(null));
        }

        [TestMethod]
        public void EmptyPrimaryModelCollection_AddAddAddSaveChangeChange_1CanSaveChangeReported()
        {
            IList<object> reportedSenders = new List<object>();
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model1 = new TestModel(1), model2 = new TestModel(2);

            collection.AddNewModel(model1);
            collection.AddNewModel(model2);
            collection.AddNewModel(new TestModel(3));
            collection.Save.Execute(null);
            Assert.IsFalse(collection.Save.CanExecute(null));
            collection.Save.CanExecuteChanged += (sender, e) => reportedSenders.Add(sender);

            model1.Property += 1;
            Assert.AreEqual(1, reportedSenders.Count);
            Assert.IsTrue(collection.Save.CanExecute(null));
            model2.Property += 1;
            Assert.AreEqual(1, reportedSenders.Count);
            Assert.IsTrue(collection.Save.CanExecute(null));
        }

        [TestMethod]
        public void HasModelReturnsFalseForEmptyCollection()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            Assert.IsFalse(collection.HasModel(Guid.NewGuid()));
        }

        [TestMethod]
        public void HasModelReturnsTrueForModelsInCollection()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            int numberOfModels = 10;
            Guid[] modelGuids = new Guid[numberOfModels];
            for (int i = 0; i < numberOfModels; i++)
            {
                modelGuids[i] = collection.AddNewModel(new TestModel());
            }
            foreach (Guid guid in modelGuids)
            {
                Assert.IsTrue(collection.HasModel(guid));
            }
        }

        [TestMethod]
        public void HasModelReturnsFalseForModelsNotInCollection()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            int numberOfModels = 10;
            for (int i = 0; i < numberOfModels; i++)
            {
                collection.AddNewModel(new TestModel());
            }
            Assert.IsFalse(collection.HasModel(Guid.NewGuid()));
        }
    }
}
