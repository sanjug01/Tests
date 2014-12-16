using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;

namespace RdClient.Shared.Test.Model
{

    [TestClass]
    public class ModelCollectionTests
    {
        private TestData _testData = new TestData();
        private ModelCollection<ModelBase> _collection = new ModelCollection<ModelBase>();

        [TestInitialize]
        public void TestSetup()
        {            
            foreach(ModelBase item in _testData.NewListOfModelBase())
            {
                _collection.Add(item);
            }
        }

        [TestMethod]
        public void ContainsItemWithIdReturnsTrueForItemInCollection()
        {
            ModelBase item = _collection[_testData.RandomSource.Next(0, _collection.Count)];
            Assert.IsTrue(_collection.ContainsItemWithId(item.Id));
        }

        [TestMethod]
        public void ContainsItemWithIdReturnsFalseForItemNotInCollection()
        {
            ModelBase item = new ModelBase();
            Assert.IsFalse(_collection.ContainsItemWithId(item.Id));
        }

        [TestMethod]
        public void GetItemWithIdSucceedsForItemInCollection()
        {
            ModelBase item = _collection[_testData.RandomSource.Next(0, _collection.Count)];
            ModelBase returnedItem = _collection.GetItemWithId(item.Id);
            Assert.AreSame(item, returnedItem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetItemWithIdThrowsArgumentOutOfRangeExceptionForItemNotInCollection()
        {
            ModelBase item = new ModelBase();
            _collection.GetItemWithId(item.Id);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicateItemThrowsArgumentException()
        {
            ModelBase item = _collection[_testData.RandomSource.Next(0, _collection.Count)];
            _collection.Add(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullThrowsArgumentNullException()
        {
            _collection.Add(null);
        }

        [TestMethod]
        public void RemoveFromCollectionSucceeds()
        {
            int countBeforeRemove = _collection.Count;
            ModelBase item = _collection[_testData.RandomSource.Next(0, countBeforeRemove)];
            Assert.IsTrue(_collection.Contains(item));
            Assert.IsTrue(_collection.Remove(item));
            Assert.AreEqual(countBeforeRemove - 1, _collection.Count);
            Assert.IsFalse(_collection.Contains(item));
        }

        [TestMethod]        
        public void RemoveItemNotInCollectionReturnsFalse()
        {
            int countBeforeRemove = _collection.Count;
            ModelBase item = new ModelBase();
            Assert.IsFalse(_collection.Contains(item));
            Assert.IsFalse(_collection.Remove(item));
            Assert.AreEqual(countBeforeRemove, _collection.Count);
            Assert.IsFalse(_collection.Contains(item));        
        }

        [TestMethod]
        public void RemoveAndReAddItemSucceeds()
        {
            ModelBase item = _collection[_testData.RandomSource.Next(0, _collection.Count)];
            Assert.IsTrue(_collection.Remove(item));            
            _collection.Add(item);
            Assert.IsTrue(_collection.Contains(item));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetItemThrowsInvalidOperationException()
        {
            _collection[_testData.RandomSource.Next(0, _collection.Count)] = new ModelBase();
        }
    }
}
