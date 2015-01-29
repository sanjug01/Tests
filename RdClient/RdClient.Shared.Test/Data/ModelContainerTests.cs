namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [TestClass]
    public sealed class ModelContainerTests
    {
        [TestMethod]
        public void ModelContainer_CreateForNewModel_NewContainerCreated()
        {
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(PersistentStatus.New, container.Status);
        }

        [TestMethod]
        public void ModelContainer_CreateForExistingModel_CleanContainerCreated()
        {
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(PersistentStatus.Clean, container.Status);
            Assert.AreEqual(id, container.Id);
        }

        [TestMethod]
        public void ModelContainerForNewModel_ChangeModel_StatusIsNew()
        {
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);

            model.Property += 1;

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(PersistentStatus.New, container.Status);
        }

        [TestMethod]
        public void ModelContainerForExistingModel_ChangeModel_StatusIsModified()
        {
            IList<PropertyChangedEventArgs> reportedChanges = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);
            container.PropertyChanged += (sender, e) => reportedChanges.Add(e);

            model.Property += 1;

            Assert.AreSame(model, container.Model);
            Assert.AreEqual(PersistentStatus.Modified, container.Status);
            Assert.AreEqual(1, reportedChanges.Count);
            Assert.AreEqual("Status", reportedChanges[0].PropertyName);
        }

        [TestMethod]
        public void ModelContainerForNewModel_StatusToClean_StatusSet()
        {
            IList<PropertyChangedEventArgs> reportedChanges = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);
            container.PropertyChanged += (sender, e) => reportedChanges.Add(e);

            container.Status = PersistentStatus.Clean;

            Assert.AreEqual(PersistentStatus.Clean, container.Status);
            //
            // Verify that a change from any state to Clean is not reported.
            //
            Assert.AreEqual(0, reportedChanges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ModelContainerForNewModel_StatusToModified_ExceptionThrown()
        {
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);

            container.Status = PersistentStatus.Modified;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ModelContainerForExistingModel_StatusToModified_ExceptionThrown()
        {
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);

            container.Status = PersistentStatus.Modified;
        }
    }
}
