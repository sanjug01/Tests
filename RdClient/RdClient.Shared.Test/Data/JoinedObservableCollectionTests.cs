namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using System.Collections.ObjectModel;

    [TestClass]
    public sealed class JoinedObservableCollectionTests
    {
        [TestMethod]
        public void JoinedObservableCollection_Add2EmptyCollections_Empty()
        {
            JoinedObservableCollection<int> collection = JoinedObservableCollection<int>.Create();
            ObservableCollection<int>
                c1 = new ObservableCollection<int>(),
                c2 = new ObservableCollection<int>();

            collection.AddCollection(c1);
            collection.AddCollection(c2);

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void JoinedObservableCollection_Add2Collections_ValuesCombined()
        {
            JoinedObservableCollection<int> collection = JoinedObservableCollection<int>.Create();
            ObservableCollection<int>
                c1 = new ObservableCollection<int>() { 1, 2, 3 },
                c2 = new ObservableCollection<int>() { 4, 5, 6 };

            collection.AddCollection(c1);
            collection.AddCollection(c2);

            Assert.AreEqual(c1.Count + c2.Count, collection.Count);

            foreach (int i in c1)
                CollectionAssert.Contains(collection, i);
            foreach (int i in c2)
                CollectionAssert.Contains(collection, i);
        }

        [TestMethod]
        public void JoinedObservableCollection_Add_Added()
        {
            JoinedObservableCollection<int> collection = JoinedObservableCollection<int>.Create();
            ObservableCollection<int>
                c1 = new ObservableCollection<int>() { 1, 2, 3 },
                c2 = new ObservableCollection<int>() { 4, 5, 6 };

            collection.AddCollection(c1);
            collection.AddCollection(c2);
            c2.Add(7);

            CollectionAssert.Contains(collection, 7);
        }

        [TestMethod]
        public void JoinedObservableCollection_Remove_Removed()
        {
            JoinedObservableCollection<int> collection = JoinedObservableCollection<int>.Create();
            ObservableCollection<int>
                c1 = new ObservableCollection<int>() { 1, 2, 3 },
                c2 = new ObservableCollection<int>() { 4, 5, 6 };

            collection.AddCollection(c1);
            collection.AddCollection(c2);
            c1.Remove(2);

            CollectionAssert.DoesNotContain(collection, 2);
        }

        [TestMethod]
        public void JoinedObservableCollection_Replace_Replaced()
        {
            JoinedObservableCollection<int> collection = JoinedObservableCollection<int>.Create();
            ObservableCollection<int>
                c1 = new ObservableCollection<int>() { 1, 2, 3 },
                c2 = new ObservableCollection<int>() { 4, 5, 6 };

            collection.AddCollection(c1);
            collection.AddCollection(c2);
            c1[0] = 8;

            CollectionAssert.DoesNotContain(collection, 1);
            CollectionAssert.Contains(collection, 8);
        }
    }
}
