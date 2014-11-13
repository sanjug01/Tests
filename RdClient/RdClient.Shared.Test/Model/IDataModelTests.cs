using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using RdClient.Shared.Models;
using RdClient.Shared.Test.Mock;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Model
{
    class TestData
    {
        private Random _rand = new Random();

        public Random RandomSource
        { 
            get
            {
                return _rand;
            }
        }

        public string NewRandomString()
        {
            return "rand" + RandomSource.Next();
        }

        public Desktop NewValidDesktop()
        {
            return new Desktop() { HostName = NewRandomString(), CredentialId = Guid.NewGuid() };
        }

        public List<Desktop> NewSmallListOfDesktops()
        {
            int count = RandomSource.Next(3, 10);
            List<Desktop> desktops = new List<Desktop>(count);
            for (int i = 0; i < count; i++)
            {
                desktops.Add(NewValidDesktop());
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
    }
    
    public abstract class IDataModelTests
    {
        private TestData _testData;
        private Mock.DataStorage _mockStorage;
        private IDataModel _dataModel;
        private List<Desktop> _expectedDesktops;
        private ObservableCollection<Desktop> _actualDesktops;
        private ObservableCollection<Credentials> _actualCredentials;
        private List<Credentials> _expectedCreds;

        protected abstract IDataModel GetDataModel(IDataStorage storage);

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _mockStorage = new Mock.DataStorage();
            _expectedDesktops = _testData.NewSmallListOfDesktops();
            _expectedCreds = _testData.NewSmallListOfCredentials();
            _mockStorage.Expect("LoadDesktops", new List<object>() { }, _expectedDesktops);
            _mockStorage.Expect("LoadCredentials", new List<object>() { }, _expectedCreds);
            _dataModel = GetDataModel(_mockStorage);
            _dataModel.LoadFromStorage().Wait();
            _actualDesktops = _dataModel.Desktops;
            _actualCredentials = _dataModel.Credentials;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockStorage.Dispose();
        }

        [TestMethod]
        public void LoadsDataFromStorage()
        {
            CollectionAssert.AreEqual(_expectedDesktops, _actualDesktops);
            CollectionAssert.AreEqual(_expectedCreds, _actualCredentials);
        }

        [TestMethod]
        public async Task AddDesktopSavesItToStorage()
        {
            Desktop desktop = _testData.NewValidDesktop();
            _mockStorage.Expect("SaveDesktop", new List<object>(){desktop}, 0);
            _dataModel.Desktops.Add(desktop);            
        }

        [TestMethod]        
        public void AddDuplicateDesktopThrowsException()
        {
            Desktop existingDesktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            Desktop desktopWithSameId = _testData.NewValidDesktop();
            desktopWithSameId.Id = existingDesktop.Id;
            bool gotException = false;
            try
            {
                _dataModel.Desktops.Add(desktopWithSameId);
            }
            catch (ArgumentException)
            {
                gotException = true;
            }
            Assert.IsTrue(gotException, "Did not get expected exception when adding a duplicate desktop to the DataModel");
        }

        [TestMethod]
        public void EditDesktopSavesItToStorage()
        {
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("SaveDesktop", new List<object>() { desktop }, 0);
            desktop.HostName = _testData.NewRandomString();
        }

        [TestMethod]
        public void DeleteDesktopRemovesItFromStorage()
        {
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("DeleteDesktop", new List<object>() { desktop }, 0);
            _actualDesktops.Remove(desktop);
        }

        [TestMethod]
        public void AddCredentialSavesItToStorage()
        {
            Credentials cred = _testData.NewValidCredential();
            _mockStorage.Expect("SaveCredential", new List<object>() { cred }, 0);
            _dataModel.Credentials.Add(cred);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicateCredentialThrowsException()
        {
            Credentials existingCred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            Credentials credWithSameId = _testData.NewValidCredential();
            credWithSameId.Id = existingCred.Id;
            _dataModel.Credentials.Add(credWithSameId);
        }

        [TestMethod]
        public void EditCredentialSavesItToStorage()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            _mockStorage.Expect("SaveCredential", new List<object>() { cred }, 0);
            cred.Password = _testData.NewRandomString();           
        }

        [TestMethod]
        public void DeleteCredentialRemovesItsIdFromAllDesktops()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("SaveDesktop", new List<object>() { desktop }, 0);//change credId of desktop to match cred
            desktop.CredentialId = cred.Id;

            _mockStorage.Expect("SaveDesktop", new List<object>() { desktop }, 0);//remove credId of desktop as cred is removed
            _mockStorage.Expect("DeleteCredential", new List<object>() { cred }, 0);            
            _dataModel.Credentials.Remove(cred);
        }

        [TestMethod]
        public void DeleteCredentialRemovesItFromStorage()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            _mockStorage.Expect("DeleteCredential", new List<object>() { cred }, 0);
            _dataModel.Credentials.Remove(cred);
        }

        [TestMethod]
        public void GetDesktopWithIdReturnsAddedDesktop()
        {
            Desktop expectedDesktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            Desktop actualDesktop = _dataModel.GetDesktopWithId(expectedDesktop.Id);
            Assert.AreSame(expectedDesktop, actualDesktop);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetDesktopWithIdThrowsExceptionIfIdNotAdded()
        {            
            _dataModel.GetDesktopWithId(Guid.NewGuid());
        }

        [TestMethod]
        public void GetCredentialWithIdReturnsAddedCredential()
        {
            Credentials expectedCred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            Credentials actualCred = _dataModel.GetCredentialWithId(expectedCred.Id);
            Assert.AreSame(expectedCred, actualCred);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetCredentialWithIdThrowsExceptionIfIdNotAdded()
        {
            _dataModel.GetCredentialWithId(Guid.NewGuid());
        }
    }

}
