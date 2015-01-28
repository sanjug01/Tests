namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    [TestClass]
    public sealed class OrderedModelCollectionTests
    {
        private sealed class ForwardOrder : IComparer<TestModel>
        {
            int IComparer<TestModel>.Compare(TestModel x, TestModel y)
            {
                Contract.Assert(null != x);
                Contract.Assert(null != y);
                return x.Property < y.Property ? -1 : x.Property > y.Property ? 1 : 0;
            }
        }

        private sealed class ReverseOrder : IComparer<TestModel>
        {
            int IComparer<TestModel>.Compare(TestModel x, TestModel y)
            {
                Contract.Assert(null != x);
                Contract.Assert(null != y);
                return x.Property > y.Property ? -1 : x.Property < y.Property ? 1 : 0;
            }
        }

        private IModelCollection<TestModel> _collection;

        [TestInitialize]
        public void SetUpTest()
        {
            _collection = PrimaryModelCollection<TestModel>.Load(new MemoryStorageFolder(), new TestModelSerializer());
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _collection = null;
        }

        [TestMethod]
        public void EmptySource_NewOrderedModelCollection_EmptyOrderedCollection()
        {
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);
            Assert.IsNull(orderedCollection.Order);
            Assert.IsNotNull(orderedCollection.Models);
            Assert.AreEqual(0, orderedCollection.Models.Count);
        }

        [TestMethod]
        public void NotEmptySource_NewOrderedModelCollection_EmptyOrderedCollection()
        {
            _collection.AddNewModel(new TestModel(1));
            _collection.AddNewModel(new TestModel(2));
            _collection.AddNewModel(new TestModel(3));
            _collection.AddNewModel(new TestModel(4));

            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);
            Assert.IsNull(orderedCollection.Order);
            Assert.IsNotNull(orderedCollection.Models);
            Assert.AreEqual(0, orderedCollection.Models.Count);
        }

        [TestMethod]
        public void NotEmptySource_OrderedModelCollectionSetOrder_Ordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            foreach(int i in data)
                _collection.AddNewModel(new TestModel(i));

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _collection.Models.Count);
            Assert.AreEqual(_collection.Models.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void EmptySource_OrderedModelCollectionSetOrderAddData_Ordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);
            orderedCollection.Order = order;

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));

            Assert.AreEqual(data.Length, _collection.Models.Count);
            Assert.AreEqual(_collection.Models.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedModelCollection_RemoveModel_RemovedFromOrdered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            Guid id = _collection.AddNewModel(new TestModel(18));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel removedModel = _collection.RemoveModel(id);
            VerifyStrictOrder(orderedCollection.Models, order);
            Assert.AreEqual(data.Length, orderedCollection.Models.Count);
            foreach (TestModel model in orderedCollection.Models)
                Assert.AreNotSame(removedModel, model);
            Assert.AreEqual(18, removedModel.Property);
        }

        [TestMethod]
        public void OrderedModelCollection_DuplicatesRemoveModel_RemovedFromOrdered()
        {
            int[] data = new int[] { 5, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            Guid id = _collection.AddNewModel(new TestModel(18));
            orderedCollection.Order = order;

            VerifyNonStrictOrder(orderedCollection.Models, order);

            for (int i = 0; i < 4; ++i)
            {
                TestModel removedModel = _collection.RemoveModel(_collection.Models[5].Id);
                VerifyNonStrictOrder(orderedCollection.Models, order);
                foreach (TestModel model in orderedCollection.Models)
                    Assert.AreNotSame(removedModel, model);
            }
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeFirstModelToMove_Reordered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel formerFirst = orderedCollection.Models[0];
            formerFirst.Property = 50;
            VerifyStrictOrder(orderedCollection.Models, order);
            Assert.AreNotSame(formerFirst, orderedCollection.Models[0]);
            Assert.AreSame(formerFirst, orderedCollection.Models[orderedCollection.Models.Count -1]);
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeEdgesNotToMove_OrderPreserved()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel formerFirst = orderedCollection.Models[0];

            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)orderedCollection.Models).CollectionChanged += (sender, e) => changes.Add(e);

            orderedCollection.Models[0].Property = 6;
            orderedCollection.Models[0].Property = 5;
            orderedCollection.Models[orderedCollection.Models.Count - 1].Property += 10;
            orderedCollection.Models[orderedCollection.Models.Count - 1].Property -= 10;
            Assert.AreEqual(0, changes.Count);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeMiddleNotToMove_OrderPreserved()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel formerFirst = orderedCollection.Models[0];

            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)orderedCollection.Models).CollectionChanged += (sender, e) => changes.Add(e);

            orderedCollection.Models[3].Property += 2;
            orderedCollection.Models[3].Property -= 2;
            Assert.AreEqual(0, changes.Count);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeMiddleToMoveBack_Reordered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel formerFirst = orderedCollection.Models[0];

            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)orderedCollection.Models).CollectionChanged += (sender, e) => changes.Add(e);

            orderedCollection.Models[3].Property -= 7;
            Assert.AreEqual(2, changes.Count);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeOrder_Reordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            IComparer<TestModel> order = new ForwardOrder();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            foreach (int i in data)
                _collection.AddNewModel(new TestModel(i));
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _collection.Models.Count);
            Assert.AreEqual(_collection.Models.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);

            order = new ReverseOrder();
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _collection.Models.Count);
            Assert.AreEqual(_collection.Models.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedModelCollection_ChangeOrder_PropertyChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            OrderedModelCollection<TestModel> orderedCollection = new OrderedModelCollection<TestModel>(_collection);

            orderedCollection.PropertyChanged += (sender, e) => changes.Add(e);
            orderedCollection.Order = new ForwardOrder();
            orderedCollection.Order = new ReverseOrder();
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("Order", changes[0].PropertyName);
            Assert.AreEqual("Order", changes[1].PropertyName);
        }

        private static void VerifyStrictOrder(IEnumerable<TestModel> sequence, IComparer<TestModel> order)
        {
            TestModel prevModel = null;

            foreach (TestModel model in sequence)
            {
                if (null != prevModel)
                    Assert.IsTrue(order.Compare(prevModel, model) < 0);
                prevModel = model;
            }
        }

        private static void VerifyNonStrictOrder(IEnumerable<TestModel> sequence, IComparer<TestModel> order)
        {
            TestModel prevModel = null;

            foreach (TestModel model in sequence)
            {
                if (null != prevModel)
                    Assert.IsTrue(order.Compare(prevModel, model) <= 0);
                prevModel = model;
            }
        }
    }
}
