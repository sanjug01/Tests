using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class DataModelTests
    {
        private TestData _testData;
        private Mock.DataStorage _mockStorage;
        private DataModel _dataModel;
        private List<Desktop> _expectedDesktops;
        private ObservableCollection<Desktop> _actualDesktops;
        private ObservableCollection<Credentials> _actualCredentials;
        private List<Credentials> _expectedCreds;

        [TestInitialize]
        public async void TestSetup()
        {
            _testData = new TestData();
            _mockStorage = new Mock.DataStorage();
            _expectedCreds = _testData.NewSmallListOfCredentials();
            _expectedDesktops = _testData.NewSmallListOfDesktops(_expectedCreds);
            _dataModel = new DataModel();
            Assert.IsFalse(_dataModel.Loaded);            
            _dataModel.Storage = _mockStorage;
            _mockStorage.Expect("LoadCollection", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME }, _expectedDesktops);
            _mockStorage.Expect("LoadCollection", new List<object>() { _dataModel.CREDENTIAL_COLLECTION_NAME }, _expectedCreds);
            await _dataModel.LoadFromStorage();
            Assert.IsTrue(_dataModel.Loaded);
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetStorageAfterLoadThrowsInvalidOperationException()
        {
            _dataModel.Storage = _mockStorage;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task LoadFromStorageBeforeSettingStorageThrowsInvalidOperationException()
        {
            DataModel newDataModel = new DataModel();
            await newDataModel.LoadFromStorage();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetStorageToNullThrowsArgumentNullException()
        {
            DataModel newDataModel = new DataModel();
            newDataModel.Storage = null;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetDesktopsCollectionBeforeLoadedThrowsInvalidOperationException()
        {
            DataModel newDataModel = new DataModel();
            _actualDesktops = newDataModel.Desktops;
        }

        [TestMethod]
        public void AddDesktopSavesItToStorage()
        {
            Desktop desktop = _testData.NewValidDesktop(Guid.Empty);
            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME, desktop }, 0);
            _dataModel.Desktops.Add(desktop);
        }

        [TestMethod]
        public void EditDesktopSavesItToStorage()
        {
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME, desktop }, 0);
            desktop.HostName = _testData.NewRandomString();
        }

        [TestMethod]
        public void DeleteDesktopRemovesItFromStorage()
        {
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("DeleteItem", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME, desktop }, 0);
            _actualDesktops.Remove(desktop);
        }

        [TestMethod]
        public void AddCredentialSavesItToStorage()
        {
            Credentials cred = _testData.NewValidCredential();
            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.CREDENTIAL_COLLECTION_NAME, cred }, 0);
            _dataModel.Credentials.Add(cred);
        }

        [TestMethod]
        public void EditCredentialSavesItToStorage()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.CREDENTIAL_COLLECTION_NAME, cred }, 0);
            cred.Password = _testData.NewRandomString();
        }

        [TestMethod]
        public void DeleteCredentialRemovesItsIdFromAllDesktops()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            Desktop desktop = _actualDesktops[_testData.RandomSource.Next(0, _actualDesktops.Count)];
            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME, desktop }, 0);//change credId of desktop to match cred
            desktop.CredentialId = cred.Id;

            _mockStorage.Expect("SaveItem", new List<object>() { _dataModel.DESKTOP_COLLECTION_NAME, desktop }, 0);//remove credId of desktop as cred is removed
            _mockStorage.Expect("DeleteItem", new List<object>() { _dataModel.CREDENTIAL_COLLECTION_NAME, cred }, 0);
            _dataModel.Credentials.Remove(cred);
        }

        [TestMethod]
        public void DeleteCredentialRemovesItFromStorage()
        {
            Credentials cred = _actualCredentials[_testData.RandomSource.Next(0, _actualCredentials.Count)];
            _mockStorage.Expect("DeleteItem", new List<object>() { _dataModel.CREDENTIAL_COLLECTION_NAME, cred }, 0);
            _dataModel.Credentials.Remove(cred);
        }
    }
}
