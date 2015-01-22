namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;

    [TestClass]
    public sealed class ModelContainerTests
    {
        [TestMethod]
        public void ModelContainer_CreateForNewModel_NewContainerCreated()
        {
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(ModelStatus.New, container.Status);
        }

        [TestMethod]
        public void ModelContainer_CreateForExistingModel_CleanContainerCreated()
        {
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(ModelStatus.Clean, container.Status);
            Assert.AreEqual(id, container.Id);
        }

        [TestMethod]
        public void ModelContainerForNewModel_ChangeModel_StatusIsNew()
        {
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);

            model.Property += 1;

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(ModelStatus.New, container.Status);
        }

        [TestMethod]
        public void ModelContainerForExistingModel_ChangeModel_StatusIsModified()
        {
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);

            model.Property += 1;

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(ModelStatus.Modified, container.Status);
        }
    }
}
