namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using System;
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
                Assert.AreEqual(ModelStatus.New, container.Status);
                Assert.AreSame(model, container.Model);
                Assert.AreEqual(id, container.Id);
            }
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrimaryModelCollection_RemoveNonexistingModel_ExceptionThrown()
        {
            IModelCollection<TestModel> collection = PrimaryModelCollection<TestModel>.Load(_emptyFolder, _serializer);
            TestModel model = new TestModel(1);
            collection.AddNewModel(model);
            collection.RemoveModel(Guid.NewGuid());
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
            Assert.AreEqual(ModelStatus.Clean, collection.Models[0].Status);
        }
    }
}
