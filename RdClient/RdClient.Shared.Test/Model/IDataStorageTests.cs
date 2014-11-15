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
    public abstract class IDataStorageTests
    {
        private TestData _testData = new TestData();
        private IDataStorage _dataStorage;
        private IEnumerable<string> _collectionNames;
        private string _collectionName;
        private IEnumerable<ModelBase> _collection;

        public abstract IDataStorage GetDataStorage();

        /*
         * Does some basic setup needed by all tests
         * 
         * At the end of this method, a new IDataStorage object has been created, and a single empty collection added 
         */
        [TestInitialize]
        public async Task TestSetup()
        {
            _testData = new TestData();
            _dataStorage = GetDataStorage();
            _collectionNames = await _dataStorage.GetCollectionNames();
            Assert.AreEqual(0, _collectionNames.Count());
            _collectionName = "collection" + _testData.NewRandomString();
            _collection = await _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
            _collectionNames = await _dataStorage.GetCollectionNames();
            Assert.AreEqual(1, _collectionNames.Count());
            Assert.AreEqual(_collectionName, _collectionNames.ElementAt(0));
        }

        [TestMethod]
        public async Task TestCollectionPersisted()
        {
            IDataStorage newDataStorage = GetDataStorage();
            IEnumerable<string> newCollectionNames = await newDataStorage.GetCollectionNames();
            Assert.AreEqual(1, newCollectionNames.Count());
            Assert.AreEqual(_collectionName, newCollectionNames.ElementAt(0));
        }

        [TestMethod]
        public async Task TestDeleteCollectionRemovesCollection()
        {
            await _dataStorage.DeleteCollection(_collectionName);
            IEnumerable<string> newCollectionNames = await _dataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
            IDataStorage newDataStorage = GetDataStorage();
            newCollectionNames = await newDataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
        }

        [TestMethod]
        public async Task TestDeleteCollectionReturnsTrue()
        {
            Assert.IsTrue(await _dataStorage.DeleteCollection(_collectionName));
        }

        [TestMethod]
        public async Task TestDeleteNonExistantCollectionReturnsFalse()
        {
            string secondCollectionName = _testData.NewRandomString();
            Assert.AreNotEqual(_collectionName, secondCollectionName);
            Assert.IsFalse(await _dataStorage.DeleteCollection(secondCollectionName));
        }

        [TestMethod]
        public async Task TestSaveModelBaseSucceeds()
        {
            ModelBase item = new ModelBase();
            await _dataStorage.SaveItem(_collectionName, item);
            _collection = await _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = await newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
        }

        [TestMethod]
        public async Task TestSaveDesktopSucceeds()
        {
            ModelBase item = _testData.NewValidDesktop(Guid.NewGuid());
            await _dataStorage.SaveItem(_collectionName, item);
            _collection = await _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = await newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
        }

        [TestMethod]
        public async Task TestSaveCredentialSucceeds()
        {
            ModelBase item = _testData.NewValidCredential();
            await _dataStorage.SaveItem(_collectionName, item);
            _collection = await _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = await newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
        }

        [TestMethod]
        public async Task TestSaveExistingItemOverwritesItem()
        {
            Credentials cred = _testData.NewValidCredential();
            await _dataStorage.SaveItem(_collectionName, cred);
            cred.Password = _testData.NewRandomString();
            await _dataStorage.SaveItem(_collectionName, cred);
            IDataStorage newDataStorage = GetDataStorage();
            _collection = await newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(cred, _collection.First());
        }

        [TestMethod]
        public async Task TestDeleteItemRemovesItem()
        {
            ModelBase item = new ModelBase();
            await _dataStorage.SaveItem(_collectionName, item);
            await _dataStorage.DeleteItem(_collectionName, item);
            _collection = await _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = await newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
        }

        [TestMethod]
        public async Task TestDeleteItemReturnsTrue()
        {
            ModelBase item = new ModelBase();
            await _dataStorage.SaveItem(_collectionName, item);
            Assert.IsTrue(await _dataStorage.DeleteItem(_collectionName, item));
        }

        [TestMethod]
        public async Task TestDeleteNonExistentItemReturnsFalse()
        {
            ModelBase item = new ModelBase();
            Assert.IsFalse(await _dataStorage.DeleteItem(_collectionName, item));
        }
    }
}
