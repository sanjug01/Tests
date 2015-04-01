namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    [TestClass]
    public sealed class OrderedObservableCollectionTests
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

        private ObservableCollection<TestModel> _source;
        private ReadOnlyObservableCollection<TestModel> _collection;

        [TestInitialize]
        public void SetUpTest()
        {
            _source = new ObservableCollection<TestModel>();
            _collection = new ReadOnlyObservableCollection<TestModel>(_source);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _collection = null;
        }

        [TestMethod]
        public void EmptySource_NewOrderedObservableCollection_EmptyOrderedCollection()
        {
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);
            Assert.IsNull(orderedCollection.Order);
            Assert.IsNotNull(orderedCollection.Models);
            Assert.AreEqual(0, orderedCollection.Models.Count);
        }

        [TestMethod]
        public void NotEmptySource_NewOrderedObservableCollection_EmptyOrderedCollection()
        {
            _source.Add(new TestModel(1));
            _source.Add(new TestModel(2));
            _source.Add(new TestModel(3));
            _source.Add(new TestModel(4));

            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);
            Assert.IsNull(orderedCollection.Order);
            Assert.IsNotNull(orderedCollection.Models);
            Assert.AreEqual(0, orderedCollection.Models.Count);
        }

        [TestMethod]
        public void NotEmptySource_OrderedObservableCollectionSetOrder_Ordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            foreach(int i in data)
                _source.Add(new TestModel(i));

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _collection.Count);
            Assert.AreEqual(_collection.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void EmptySource_OrderedObservableCollectionSetOrderAddData_Ordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);
            orderedCollection.Order = order;

            foreach (int i in data)
                _source.Add(new TestModel(i));

            Assert.AreEqual(data.Length, _collection.Count);
            Assert.AreEqual(_collection.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedObservableCollection_RemoveModel_RemovedFromOrdered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            TestModel removedModel = new TestModel(18);
            _source.Add(removedModel);
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            Assert.IsTrue(_source.Remove(removedModel));
            VerifyStrictOrder(orderedCollection.Models, order);
            Assert.AreEqual(data.Length, orderedCollection.Models.Count);
            foreach (TestModel m in orderedCollection.Models)
                Assert.AreNotSame(removedModel, m);
        }

        [TestMethod]
        public void OrderedObservableCollection_DuplicatesRemoveModel_RemovedFromOrdered()
        {
            int[] data = new int[] { 5, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            VerifyNonStrictOrder(orderedCollection.Models, order);

            for (int i = 0; i < 4; ++i)
            {
                TestModel removedModel = _source[5];
                _source.Remove(removedModel);
                VerifyNonStrictOrder(orderedCollection.Models, order);
                foreach (TestModel model in orderedCollection.Models)
                    Assert.AreNotSame(removedModel, model);
            }
        }

        [TestMethod]
        public void OrderedObservableCollection_ChangeFirstModelToMove_Reordered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);
            TestModel formerFirst = orderedCollection.Models[0];
            formerFirst.Property = 50;
            VerifyStrictOrder(orderedCollection.Models, order);
            Assert.AreNotSame(formerFirst, orderedCollection.Models[0]);
            Assert.AreSame(formerFirst, orderedCollection.Models[orderedCollection.Models.Count -1]);
        }

        [TestMethod]
        public void OrderedObservableCollection_ChangeEdgesNotToMove_OrderPreserved()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
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
        public void OrderedObservableCollection_ChangeMiddleNotToMove_OrderPreserved()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
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
        public void OrderedObservableCollection_ChangeMiddleToMoveBack_Reordered()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
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
        public void OrderedObservableCollection_MoveElement_NoChanges()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);

            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)orderedCollection.Models).CollectionChanged += (sender, e) => changes.Add(e);

            _source.Move(0, 5);
            Assert.AreEqual(0, changes.Count);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedObservableCollection_ReplaceElement_Rebuilt()
        {
            int[] data = new int[] { 5, 10, 15, 20, 25, 30, 35 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            VerifyStrictOrder(orderedCollection.Models, order);

            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)orderedCollection.Models).CollectionChanged += (sender, e) => changes.Add(e);

            _source[2] = new TestModel(50);
            Assert.AreNotEqual(0, changes.Count);
            Assert.AreEqual(data.Length, orderedCollection.Models.Count);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedObservableCollection_ChangeOrder_Reordered()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _source.Count);
            Assert.AreEqual(_source.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);

            order = new ReverseOrder();
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _source.Count);
            Assert.AreEqual(_source.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);
        }

        [TestMethod]
        public void OrderedObservableCollection_ClearOrder_Cleared()
        {
            int[] data = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 50, 5 };

            IComparer<TestModel> order = new ForwardOrder();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_collection);

            foreach (int i in data)
                _source.Add(new TestModel(i));
            orderedCollection.Order = order;

            Assert.AreEqual(data.Length, _source.Count);
            Assert.AreEqual(_source.Count, orderedCollection.Models.Count);
            Assert.AreSame(order, orderedCollection.Order);
            VerifyStrictOrder(orderedCollection.Models, order);

            orderedCollection.Order = null;

            Assert.AreEqual(0, orderedCollection.Models.Count);
            Assert.IsNull(orderedCollection.Order);
        }

        [TestMethod]
        public void OrderedObservableCollection_ChangeOrder_PropertyChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            IOrderedObservableCollection<TestModel> orderedCollection = OrderedObservableCollection<TestModel>.Create(_source);

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
