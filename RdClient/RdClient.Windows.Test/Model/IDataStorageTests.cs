using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using RdClient.Shared.Models;
using RdClient.Models;

namespace RdClient.Windows.Test.Model
{    
    public abstract class IDataStorageTests
    {
#if false
        private TestData _testData = new TestData();
        private IDataStorage _dataStorage;
        private IEnumerable<string> _collectionNames;
        private string _collectionName;
        private IEnumerable<ModelBase> _collection;

        public abstract IDataStorage GetDataStorage();

        public abstract void DataStorageSetup();

        public abstract void DataStorageCleanup();

        /*
         * Does some basic setup needed by all tests
         * 
         * At the end of this method, a new IDataStorage object has been created, and a single empty collection added 
         */
        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            DataStorageSetup();
            _dataStorage = GetDataStorage();
            _collectionNames = _dataStorage.GetCollectionNames();
            Assert.AreEqual(0, _collectionNames.Count());
            _collectionName = "collection" + _testData.NewRandomString();
            _collection = _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
            _collectionNames = _dataStorage.GetCollectionNames();
            Assert.AreEqual(1, _collectionNames.Count());
            Assert.AreEqual(_collectionName, _collectionNames.ElementAt(0));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataStorageCleanup();
        }

        [TestMethod]
        public void TestCollectionPersisted()
        {
            IDataStorage newDataStorage = GetDataStorage();
            IEnumerable<string> newCollectionNames = newDataStorage.GetCollectionNames();
            Assert.AreEqual(1, newCollectionNames.Count());
            Assert.AreEqual(_collectionName, newCollectionNames.ElementAt(0));
        }

        [TestMethod]
        public void TestDeleteEmptyCollectionRemovesCollection()
        {
            _dataStorage.DeleteCollection(_collectionName);
            IEnumerable<string> newCollectionNames = _dataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
            IDataStorage newDataStorage = GetDataStorage();
            newCollectionNames = newDataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
        }

        [TestMethod]
        public void TestDeleteNonEmptyCollectionRemovesCollection()
        {
            _dataStorage.SaveItem(_collectionName, new ModelBase());
            _dataStorage.DeleteCollection(_collectionName);
            IEnumerable<string> newCollectionNames = _dataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
            IDataStorage newDataStorage = GetDataStorage();
            newCollectionNames = newDataStorage.GetCollectionNames();
            Assert.AreEqual(0, newCollectionNames.Count());
        }

        [TestMethod]
        public void TestDeleteCollectionReturnsTrue()
        {
            _dataStorage.DeleteCollection(_collectionName);
        }

        [TestMethod]
        public void TestDeleteNonExistantCollectionReturnsFalse()
        {
            string secondCollectionName = _testData.NewRandomString();
            Assert.AreNotEqual(_collectionName, secondCollectionName);
            bool exceptionThrown = false;

            try
            {
                _dataStorage.DeleteCollection(secondCollectionName);
            }
            catch(SerializationException /* e */)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void TestSaveModelBaseSucceeds()
        {
            ModelBase item = new ModelBase();
            _dataStorage.SaveItem(_collectionName, item);
            _collection = _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Assert.AreEqual(item, _collection.First());
        }

        [TestMethod]
        public void TestDesktopPersisted()
        {
            Desktop desktop = _testData.NewValidDesktop(Guid.NewGuid());
            _dataStorage.SaveItem(_collectionName, desktop);
            IDataStorage newDataStorage = GetDataStorage();
            _collection = newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Desktop retreivedDesktop = _collection.First() as Desktop;
            Assert.AreEqual(desktop, retreivedDesktop);
            Assert.AreEqual(desktop.HostName, retreivedDesktop.HostName);
            Assert.AreEqual(desktop.CredentialId, retreivedDesktop.CredentialId);
        }

        [TestMethod]
        public void TestCredentialPersisted()
        {
            Credentials cred = _testData.NewValidCredential();
            _dataStorage.SaveItem(_collectionName, cred);
            IDataStorage newDataStorage = GetDataStorage();
            _collection = newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Credentials retreivedCred = _collection.First() as Credentials;
            Assert.AreEqual(cred, retreivedCred);
            Assert.AreEqual(cred.Domain, retreivedCred.Domain);
            Assert.AreEqual(cred.Username, retreivedCred.Username);
            Assert.AreEqual(cred.Password, retreivedCred.Password);
            Assert.AreEqual(cred.HaveBeenPersisted, retreivedCred.HaveBeenPersisted);
        }

        [TestMethod]
        public void TestSaveExistingItemOverwritesItem()
        {
            Credentials cred = _testData.NewValidCredential();
            _dataStorage.SaveItem(_collectionName, cred);
            cred.Password = _testData.NewRandomString();
            _dataStorage.SaveItem(_collectionName, cred);
            IDataStorage newDataStorage = GetDataStorage();
            _collection = newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(1, _collection.Count());
            Credentials retreivedCred = _collection.First() as Credentials;
            Assert.AreEqual(cred.Password, retreivedCred.Password);
        }

        [TestMethod]
        public void TestDeleteItemRemovesItem()
        {
            ModelBase item = new ModelBase();
            _dataStorage.SaveItem(_collectionName, item);
            _dataStorage.DeleteItem(_collectionName, item);
            _collection = _dataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
            IDataStorage newDataStorage = GetDataStorage();
            _collection = newDataStorage.LoadCollection(_collectionName);
            Assert.AreEqual(0, _collection.Count());
        }

        [TestMethod]
        public void TestDeleteItemReturnsTrue()
        {
            ModelBase item = new ModelBase();
            _dataStorage.SaveItem(_collectionName, item);
            _dataStorage.DeleteItem(_collectionName, item);
        }

        [TestMethod]
        public void TestDeleteNonExistentItemReturnsFalse()
        {
            ModelBase item = new ModelBase();
            bool exceptionThrown = false;
            try 
            { 
                _dataStorage.DeleteItem(_collectionName, item);
            }
            catch(SerializationException /* e */)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }
    }

    public class TestData
    {
        private Random _rand = new Random();

        public Random RandomSource
        {
            get
            {
                return _rand;
            }
        }

        public List<ModelBase> NewListOfModelBase()
        {
            int count = RandomSource.Next(3, 10);
            List<ModelBase> result = new List<ModelBase>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(new ModelBase());
            }
            return result;
        }

        public string NewRandomString()
        {
            return "rand" + RandomSource.Next();
        }

        public ModelBase NewValidModelBaseOrSubclass()
        {
            int numClasses = 3;
            int random = RandomSource.Next(numClasses);
            switch (random)
            {
                case 0:
                    return NewValidDesktop(Guid.NewGuid());
                case 1:
                    return NewValidCredential();
                default:
                    return new ModelBase();
            }
        }

        public Desktop NewValidDesktop(Guid credId)
        {
            return new Desktop() { HostName = NewRandomString(), CredentialId = credId };
        }

        public List<Desktop> NewSmallListOfDesktops(List<Credentials> creds)
        {
            int count = RandomSource.Next(3, 10);
            List<Desktop> desktops = new List<Desktop>(count);
            for (int i = 0; i < count; i++)
            {
                Guid credId = creds[_rand.Next(0, creds.Count)].Id;
                desktops.Add(NewValidDesktop(credId));
            }
            return desktops;
        }

        public Credentials NewValidCredential()
        {
            return new Credentials()
            {
                Domain = NewRandomString(),
                Username = NewRandomString(),
                Password = NewRandomString()
            };
        }

        public List<Credentials> NewSmallListOfCredentials()
        {
            int count = RandomSource.Next(3, 10);
            List<Credentials> creds = new List<Credentials>(count);
            for (int i = 0; i < count; i++)
            {
                creds.Add(NewValidCredential());
            }
            return creds;
        }
#endif
    }
}
