namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public sealed class FilteringObservableCollectionTests
    {
        private ObservableCollection<int> _source;

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
            _source = new ObservableCollection<int>();
        }

        [TestMethod]
        public void NewFilteringObservableCollection_EmptySource_Empty()
        {
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => true);
            Assert.IsNotNull(filtered);
            Assert.AreEqual(0, filtered.Count);
        }

        [TestMethod]
        public void NewFilteringObservableCollection_NonEmptySourceAllExcluded_Empty()
        {
            _source.Add(1);
            _source.Add(2);
            _source.Add(3);

            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => false);

            Assert.IsNotNull(filtered);
            Assert.AreEqual(0, filtered.Count);
        }

        [TestMethod]
        public void NewFilteringObservableCollection_NonEmptySourceAllIncluded_AllIn()
        {
            _source.Add(1);
            _source.Add(2);
            _source.Add(3);

            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => true);

            Assert.IsNotNull(filtered);
            Assert.AreEqual(_source.Count, filtered.Count);
            foreach (int n in _source)
                CollectionAssert.Contains(filtered, n);
        }

        [TestMethod]
        public void NewFilteringObservableCollection_NonEmptySourceSomeIncluded_Filtered()
        {
            _source.Add(1);
            _source.Add(2);
            _source.Add(3);

            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);

            Assert.IsNotNull(filtered);
            Assert.AreEqual(2, filtered.Count);
            CollectionAssert.Contains(filtered, 1);
            CollectionAssert.Contains(filtered, 2);
            CollectionAssert.DoesNotContain(filtered, 3);
        }

        [TestMethod]
        public void NewFilteringObservableCollection_AddToSourceSomeIncluded_Filtered()
        {
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);

            Assert.AreEqual(2, filtered.Count);
            CollectionAssert.Contains(filtered, 1);
            CollectionAssert.Contains(filtered, 2);
            CollectionAssert.DoesNotContain(filtered, 3);
        }

        [TestMethod]
        public void FilteringObservableCollection_RemoveExcluded_NoChanges()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source.Remove(3);

            Assert.AreEqual(0, changes.Count);
            Assert.AreEqual(2, filtered.Count);
            CollectionAssert.Contains(filtered, 1);
            CollectionAssert.Contains(filtered, 2);
            CollectionAssert.DoesNotContain(filtered, 3);
            CollectionAssert.DoesNotContain(filtered, 4);
        }

        [TestMethod]
        public void FilteringObservableCollection_RemoveIncluded_Changes()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source.Remove(1);

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, filtered.Count);
            CollectionAssert.DoesNotContain(filtered, 1);
            CollectionAssert.Contains(filtered, 2);
            CollectionAssert.DoesNotContain(filtered, 3);
            CollectionAssert.DoesNotContain(filtered, 4);
        }

        [TestMethod]
        public void FilteringObservableCollection_ReplaceIncludedWithIncluded_Replaced()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source[0] = 0;

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(2, filtered.Count);
            Assert.AreEqual(0, filtered[0]);
        }

        [TestMethod]
        public void FilteringObservableCollection_ReplaceIncludedWithExcluded_Removed()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source[0] = 5;

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, filtered.Count);
            CollectionAssert.DoesNotContain(filtered, 1);
        }

        [TestMethod]
        public void FilteringObservableCollection_ReplaceExcludedWithIncluded_Added()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source[2] = 0;

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(3, filtered.Count);
            CollectionAssert.Contains(filtered, 0);
        }

        [TestMethod]
        public void FilteringObservableCollection_ReplaceExcludedWithExcluded_NoChanges()
        {
            IList<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(_source, n => n < 3);
            INotifyCollectionChanged ncc = filtered;

            _source.Add(1);
            _source.Add(2);
            _source.Add(3);
            _source.Add(4);

            ncc.CollectionChanged += (sender, e) => changes.Add(e);
            _source[2] = 6;

            Assert.AreEqual(0, changes.Count);
            CollectionAssert.DoesNotContain(filtered, 6);
        }

        [TestMethod]
        public void FilteringObservableCollection_ResetSource_FilteredNewData()
        {
            ResettableObservableList<int> rol = new ResettableObservableList<int>() { 1, 2, 3, 4, 5 };
            ReadOnlyObservableCollection<int> filtered = FilteringObservableCollection<int>.Create(rol, n => n < 3);

            Assert.AreEqual(2, filtered.Count);
            CollectionAssert.Contains(filtered, 1);
            CollectionAssert.Contains(filtered, 2);

            rol.Reset(new int[] { 4, 5, 6, 0, -1, -2 });

            Assert.AreEqual(3, filtered.Count);
            CollectionAssert.Contains(filtered, 0);
            CollectionAssert.Contains(filtered, -1);
            CollectionAssert.Contains(filtered, -2);
        }
    }
}
