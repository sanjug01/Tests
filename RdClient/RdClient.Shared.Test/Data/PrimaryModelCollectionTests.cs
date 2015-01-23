namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using System;

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
    }
}
