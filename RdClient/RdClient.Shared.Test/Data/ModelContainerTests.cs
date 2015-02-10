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
        public void ModelContainerForExistingModel_ChangeModelSetContainerClean_BothAreClean()
        {
            IList<PropertyChangedEventArgs> reportedChanges = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel(10);
            Guid id = Guid.NewGuid();
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForExistingModel(id, model);
            container.PropertyChanged += (sender, e) => reportedChanges.Add(e);

            model.Property += 1;
            container.SetClean();

            Assert.AreEqual(PersistentStatus.Clean, container.Status);
            Assert.AreEqual(PersistentStatus.Clean, container.Model.Status);
        }

        [TestMethod]
        public void ModelContainerForNewModel_StatusToClean_StatusSet()
        {
            IList<PropertyChangedEventArgs> reportedChanges = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel(10);
            IModelContainer<TestModel> container = ModelContainer<TestModel>.CreateForNewModel(model);
            container.PropertyChanged += (sender, e) => reportedChanges.Add(e);

            model.Property += 1;
            container.SetClean();

            Assert.AreEqual(PersistentStatus.Clean, container.Status);
            Assert.AreEqual(PersistentStatus.Clean, container.Model.Status);
            //
            // Verify that a change from any state to Clean is not reported.
            //
            Assert.AreEqual(0, reportedChanges.Count);
        }
    }
}
