namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using System.Collections.ObjectModel;

    [TestClass]
    public sealed class TransformingObservableCollectionTests
    {
        private ObservableCollection<int> _trueSource;
        private ReadOnlyObservableCollection<int> _source;

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
    }
}
