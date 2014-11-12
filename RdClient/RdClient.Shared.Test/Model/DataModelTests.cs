using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using RdClient.Shared.Models;
using RdClient.Shared.Test.Mock;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public abstract class DataModelTests
    {
        protected abstract IDataModel GetDataModel(IDataStorage storage);

        [TestMethod]
        public void LoadsDataFromStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddDesktopSavesItToStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddDesktopFailsIfItIsAlreadyAdded()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EditDesktopSavesItToStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteDesktopRemovesItFromStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddCredentialSavesItToStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddCredentialFailsIfItIsAlreadyAdded()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EditCredentialSavesItToStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteCredentialRemovesItsIdFromAllDesktops()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteCredentialRemovesItFromStorage()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetDesktopWithIdReturnsAddedDesktop()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetDesktopWithIdThrowsExceptionIfIdNotAdded()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetCredentialWithIdReturnsAddedCredential()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetCredentialWithIdThrowsExceptionIfIdNotAdded()
        {
            throw new NotImplementedException();
        }
    }
}
