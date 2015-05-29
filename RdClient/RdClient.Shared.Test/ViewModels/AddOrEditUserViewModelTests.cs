namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class AddOrEditUserViewModelTests
    {
        private TestData _testData;
        private Mock.ModalPresentationContext _context;
        private Mock.NavigationService _nav;
        private AddOrEditUserViewArgs _args;
        private AddOrEditUserViewModel _vm;
        private ApplicationDataModel _dataModel;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();
            IList<IModelContainer<CredentialsModel>> creds = _testData.NewSmallListOfCredentials();
            foreach (IModelContainer<CredentialsModel> cred in creds)
            {
                _dataModel.Credentials.AddNewModel(cred.Model);
            }

            _nav = new Mock.NavigationService();
            _context = new Mock.ModalPresentationContext();
            var user = _dataModel.Credentials.Models[_testData.RandomSource.Next(0, _dataModel.Credentials.Models.Count)];
            _args = AddOrEditUserViewArgs.EditUser(user);
            _vm = new AddOrEditUserViewModel();
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((IViewModel)_vm).Presenting(_nav, _args, _context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _nav.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void PropertiesSetToArgs()
        {
            Assert.AreEqual(_args.CanDelete, _vm.CanDelete);
            Assert.AreEqual(_args.Mode, _vm.Mode);
            Assert.AreEqual(_args.Save, _vm.StoreCredentials);
            Assert.AreEqual(_args.ShowMessage, _vm.ShowMessage);
            Assert.AreEqual(_args.ShowSave, _vm.ShowSave);
            Assert.AreEqual(_args.Credentials.Model.Username, _vm.User.Value);
            Assert.AreEqual(_args.Credentials.Model.Password, _vm.Password);
        }

        [TestMethod]
        public void ShouldSetStoreCredentials()
        {            
            bool newValue = !_vm.StoreCredentials;
            _vm.StoreCredentials = newValue;
            Assert.AreEqual(newValue, _vm.StoreCredentials);
        }

        [TestMethod]
        public void SetPasswordDoesNotChangePassedInCredentials()
        {
            string password = _args.Credentials.Model.Password;
            string newPassword = password + _testData.NewRandomString();
            Assert.AreNotEqual(password, newPassword, "Test precondition - these should be different");

            _vm.Password = newPassword;

            Assert.AreEqual(newPassword, _vm.Password); //password should be set
            Assert.AreEqual(password, _args.Credentials.Model.Password); //should not change passed in credentials
        }

        [TestMethod]
        public void SetUserValueDoesNotChangePassedInCredentials()
        {
            string username = _args.Credentials.Model.Username;
            string newUsername = username + _testData.NewRandomString();
            Assert.AreNotEqual(username, newUsername, "Test precondition - these should be different");

            _vm.User.Value = newUsername;

            Assert.AreEqual(newUsername, _vm.User.Value); //user should be set
            Assert.AreEqual(username, _args.Credentials.Model.Username); //should not change passed in credentials
        }

        [TestMethod]
        public void UserValidForNonEmptyEmptyUsername()
        {
            _vm.User.Value = "Don Pedro";
            Assert.AreEqual(ValidationResultStatus.Valid, _vm.User.State.Status);
        }

        [TestMethod]
        public void UserValidForUserWithNonAlphanumericCharacters()
        {
            _vm.User.Value = "!";
            Assert.AreEqual(ValidationResultStatus.Valid, _vm.User.State.Status);
        }

        [TestMethod]
        public void UserValidForSameUsername()
        {
            Assert.AreEqual(ValidationResultStatus.Valid, _vm.User.State.Status);
        }

        [TestMethod]
        public void UserEmptyForEmptyUsername()
        {
            _vm.User.Value = string.Empty;
            Assert.AreEqual(ValidationResultStatus.Empty, _vm.User.State.Status);
        }

        [TestMethod]
        public void UserEmptyForNullUsername()
        {
            _vm.User.Value = null;
            Assert.AreEqual(ValidationResultStatus.Empty, _vm.User.State.Status);
        }

        [TestMethod]
        public void UserInvalidForDuplicateUsername()
        {
            var otherUsers = _dataModel.Credentials.Models.Where(m => m.Id != _args.Credentials.Id).ToList();
            var otherUser = otherUsers[_testData.RandomSource.Next(0, otherUsers.Count)];
            _vm.User.Value = DuplicateUsername();
            Assert.AreEqual(ValidationResultStatus.Invalid, _vm.User.State.Status);
        }

        [TestMethod]
        public void OkCommandCanExecuteFalseForEmptyUsername()
        {
            _vm.User.Value = "";
            _vm.Password = "secret";
            Assert.IsFalse(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void OkCommandCanExecuteTrueForNonEmptyUsername()
        {
            _vm.User.Value = DuplicateUsername(); //CanExecute should still be true for this invalid duplicate username
            _vm.Password = "";//CanExecute should still be true for empty password
            Assert.IsTrue(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void OkCommandDoesNothingForEmptyUsername()
        {
            _vm.User.Value = "";
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void OkCommandDoesNothingForInvalidUsername()
        {
            _vm.User.Value = DuplicateUsername();
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void OkShouldCallCompletionHandlerAndSaveToDatamodel()
        {
            _args = AddOrEditUserViewArgs.AddUser();
            _vm = new AddOrEditUserViewModel();
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((IViewModel)_vm).Presenting(_nav, _args, _context);

            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User.Value = newCreds.Username;
            _vm.Password = newCreds.Password;
            _vm.StoreCredentials = true;
            var previousCredCount = _dataModel.Credentials.Models.Count;

            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsFalse(result.UserCancelled);
                Assert.IsFalse(result.Deleted);
                Assert.IsTrue(result.Saved);
                Assert.IsNotNull(result.Credentials);
                Assert.AreEqual(previousCredCount + 1, _dataModel.Credentials.Models.Count);
                Assert.AreNotEqual(Guid.Empty, result.Credentials.Id);
                Assert.AreEqual(result.Credentials.Model, _dataModel.Credentials.GetModel(result.Credentials.Id));
                Assert.AreEqual(newCreds.Username, result.Credentials.Model.Username);
                Assert.AreEqual(newCreds.Password, result.Credentials.Model.Password);
                return null;
            });
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void OkShouldCallCompletionHandlerAndNotSaveToDatamodel()
        {
            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User.Value = newCreds.Username;
            _vm.Password = newCreds.Password;
            _vm.StoreCredentials = false;
            var previousCredCount = _dataModel.Credentials.Models.Count;

            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsFalse(result.UserCancelled);
                Assert.IsFalse(result.Deleted);
                Assert.IsFalse(result.Saved);
                Assert.IsNotNull(result.Credentials);
                Assert.AreEqual(_args.Credentials, result.Credentials);
                Assert.AreEqual(previousCredCount, _dataModel.Credentials.Models.Count);
                Assert.AreEqual(newCreds.Username, result.Credentials.Model.Username);
                Assert.AreEqual(newCreds.Password, result.Credentials.Model.Password);
                return null;
            });
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void CancelShouldCallCompletionHandlerAndNotSaveCredentials()
        {
            CredentialsModel originalCreds = new CredentialsModel(_args.Credentials.Model);
            var previousCredCount = _dataModel.Credentials.Models.Count;
            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User.Value = newCreds.Username;
            _vm.Password = newCreds.Password;
            _vm.StoreCredentials = true;

            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsTrue(result.UserCancelled);
                Assert.IsFalse(result.Saved);
                Assert.IsFalse(result.Deleted);
                Assert.IsNull(result.Credentials);
                Assert.AreEqual(previousCredCount, _dataModel.Credentials.Models.Count);
                Assert.AreEqual(originalCreds.Username, _args.Credentials.Model.Username);
                Assert.AreEqual(originalCreds.Password, _args.Credentials.Model.Password);
                return null;
            });
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        public void DeleteShouldCallCompletionHandlerAndDeleteCredentials()
        {
            CredentialsModel originalCreds = new CredentialsModel(_args.Credentials.Model);
            var previousCredCount = _dataModel.Credentials.Models.Count;
            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User.Value = newCreds.Username;
            _vm.Password = newCreds.Password;
            _vm.StoreCredentials = true;

            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsTrue(result.UserCancelled);
                Assert.IsFalse(result.Saved);
                Assert.IsTrue(result.Deleted);
                Assert.IsNull(result.Credentials);
                Assert.IsFalse(_dataModel.Credentials.HasModel(_args.Credentials.Id));
                Assert.AreEqual(previousCredCount - 1, _dataModel.Credentials.Models.Count);
                Assert.AreEqual(originalCreds.Username, _args.Credentials.Model.Username);
                Assert.AreEqual(originalCreds.Password, _args.Credentials.Model.Password);
                return null;
            });
            _vm.Delete.Execute(null);
        }

        private string DuplicateUsername()
        {
            var otherUsers = _dataModel.Credentials.Models.Where(m => m.Id != _args.Credentials.Id).ToList();
            var otherUser = otherUsers[_testData.RandomSource.Next(0, otherUsers.Count)];
            return otherUser.Model.Username;
        }
    }
}
