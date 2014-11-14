using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using RdClient.Shared.Models;
using RdClient.Shared.Test.Mock;
using RdClient.Shared.Test.Helpers;

namespace RdClient.Shared.Test.Model
{    
    public abstract class IDataModelTests
    {
        private TestData _testData;
        private Mock.DataStorage _mockStorage;
        private IDataModel _dataModel;
        private List<Desktop> _expectedDesktops;
        private ObservableCollection<Desktop> _actualDesktops;
        private ObservableCollection<Credentials> _actualCredentials;
        private List<Credentials> _expectedCreds;

        protected abstract Task<IDataModel> CreateDataModel(IDataStorage storage);

        [TestInitialize]
        public async void TestSetup()
        {
            _testData = new TestData();
            _mockStorage = new Mock.DataStorage();            
            _expectedCreds = _testData.NewSmallListOfCredentials();
            _expectedDesktops = _testData.NewSmallListOfDesktops(_expectedCreds);
            _mockStorage.Expect("LoadDesktops", new List<object>() { }, _expectedDesktops);
            _mockStorage.Expect("LoadCredentials", new List<object>() { }, _expectedCreds);
            _dataModel = await CreateDataModel(_mockStorage);
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
        public void AddDesktopSavesItToStorage()
        {
            Desktop desktop = _testData.NewValidDesktop(Guid.Empty);
            _mockStorage.Expect("SaveDesktop", new List<object>(){desktop}, 0);
            _dataModel.Desktops.Add(desktop);            
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
    }

}
