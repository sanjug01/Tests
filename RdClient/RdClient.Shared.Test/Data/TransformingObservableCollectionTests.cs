namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    [TestClass]
    public sealed class TransformingObservableCollectionTests
    {
        private ObservableCollection<int> _trueSource;
        private ReadOnlyObservableCollection<int> _source;

        private sealed class ResettableObservableList<T> : List<T>, INotifyCollectionChanged
        {
            private NotifyCollectionChangedEventHandler _handler;

            public void Reset(IEnumerable<T> newContents)
            {
                this.Clear();
                this.InsertRange(0, newContents);
                if (null != _handler)
                    _handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
            {
                add { _handler += value; }
                remove { _handler -= value; }
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _trueSource = new ObservableCollection<int>();
            _source = new ReadOnlyObservableCollection<int>(_trueSource);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _source = null;
            _trueSource = null;
        }

        [TestMethod]
        public void NewTransformingObservableCollection_EmptySource_EmptyCollection()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            Assert.AreEqual(0, transformed.Count);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_NonEmptySource_PopulatedCollection()
        {
            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("1", transformed[0]);
            Assert.AreEqual("2", transformed[1]);
            Assert.AreEqual("3", transformed[2]);
            Assert.AreEqual("4", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_EmptySourceAdd_AddedTransformedValue()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("1", transformed[0]);
            Assert.AreEqual("2", transformed[1]);
            Assert.AreEqual("3", transformed[2]);
            Assert.AreEqual("4", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_AddRemove_AddedRemovedTransformedValue()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource.RemoveAt(0);

            Assert.AreEqual(3, transformed.Count);
            Assert.AreEqual("2", transformed[0]);
            Assert.AreEqual("3", transformed[1]);
            Assert.AreEqual("4", transformed[2]);

            _trueSource.RemoveAt(1);

            Assert.AreEqual(2, transformed.Count);
            Assert.AreEqual("2", transformed[0]);
            Assert.AreEqual("4", transformed[1]);

            _trueSource.RemoveAt(1);

            Assert.AreEqual(1, transformed.Count);
            Assert.AreEqual("2", transformed[0]);
        }

        [TestMethod]
        public void NewTransformingObservableCollectionWithRemovedHandler_Remove_RemovedHandlerCalled()
        {
            List<string> removed = new List<string>();
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>
                .Create(_source, number => number.ToString(), str => removed.Add(str));

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource.RemoveAt(0);
            CollectionAssert.Contains(removed, "1");
        }

        [TestMethod]
        public void NewTransformingObservableCollection_MoveBackward_MovedTransformedValue()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource.Move(2, 0);

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("3", transformed[0]);
            Assert.AreEqual("1", transformed[1]);
            Assert.AreEqual("2", transformed[2]);
            Assert.AreEqual("4", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_MoveForward_MovedTransformedValue()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource.Move(1, 3);

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("1", transformed[0]);
            Assert.AreEqual("3", transformed[1]);
            Assert.AreEqual("4", transformed[2]);
            Assert.AreEqual("2", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_Replace_ReplacedTransformed()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource[2] = 5;

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("1", transformed[0]);
            Assert.AreEqual("2", transformed[1]);
            Assert.AreEqual("5", transformed[2]);
            Assert.AreEqual("4", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_Clear_ClearedTransformed()
        {
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(_source, number => number.ToString());

            _trueSource.Add(1);
            _trueSource.Add(2);
            _trueSource.Add(3);
            _trueSource.Add(4);

            _trueSource.Clear();

            Assert.AreEqual(0, transformed.Count);
        }

        [TestMethod]
        public void NewTransformingObservableCollection_Reset_PopulatedWithNewValues()
        {
            ResettableObservableList<int> resettableSource = new ResettableObservableList<int>() { 1, 2, 3, 4 };
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>.Create(resettableSource, number => number.ToString());

            Assert.AreEqual(4, transformed.Count);

            resettableSource.Reset(new int[] { 5, 6, 7, 8 });

            Assert.AreEqual(4, transformed.Count);
            Assert.AreEqual("5", transformed[0]);
            Assert.AreEqual("6", transformed[1]);
            Assert.AreEqual("7", transformed[2]);
            Assert.AreEqual("8", transformed[3]);
        }

        [TestMethod]
        public void NewTransformingObservableCollectionWithRemovedHandler_Reset_EverythingRemoved()
        {
            List<string> removed = new List<string>();
            ResettableObservableList<int> resettableSource = new ResettableObservableList<int>() { 1, 2, 3, 4 };
            ReadOnlyObservableCollection<string> transformed = TransformingObservableCollection<int, string>
                .Create(resettableSource, number => number.ToString(), str => removed.Add(str));

            resettableSource.Reset(new int[] { 5, 6, 7, 8 });
            Assert.AreEqual(4, removed.Count);
            CollectionAssert.Contains(removed, "1");
            CollectionAssert.Contains(removed, "2");
            CollectionAssert.Contains(removed, "3");
            CollectionAssert.Contains(removed, "4");
        }
    }
}
