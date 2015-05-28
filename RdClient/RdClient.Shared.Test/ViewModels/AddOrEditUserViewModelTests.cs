namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
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
        private AddUserViewArgs _args;
        private AddOrEditUserViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _nav = new Mock.NavigationService();
            _context = new Mock.ModalPresentationContext();            
            _args = new AddUserViewArgs(_testData.NewValidCredential().Model, true, CredentialPromptMode.FreshCredentialsNeeded);
            _vm = new AddOrEditUserViewModel();
            ((IViewModel)_vm).Presenting(_nav, _args, _context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _nav.Dispose();
        }

        [TestMethod]
        public void ShouldSetStoreCredentials()
        {
            _vm.StoreCredentials = true;
            Assert.IsTrue(_vm.StoreCredentials);
        }

        [TestMethod]
        public void UserValidForNonEmptyEmptyUsername()
        {
            _vm.User.Value = "Don Pedro";
            Assert.IsTrue(_vm.User.State.Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void UserValidForUserWithNonAlphanumericCharacters()
        {
            _vm.User.Value = "!";
            Assert.IsTrue(_vm.User.State.Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void UserNotValidForEmptyUsername()
        {
            _vm.User.Value = "";
            Assert.IsFalse(_vm.User.State.Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void OkCommandCanExecuteTrueForValidUsernameAndPassword()
        {
            _vm.User.Value = "Don Pedro";
            _vm.Password = "secret";
            Assert.IsTrue(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void OkCommandCanExecuteFalseForNullUsername()
        {
            _vm.User.Value = null;
            _vm.Password = "secret";
            Assert.IsFalse(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void OkCommandCanExecuteTrueForValidUsernameAndNullPassword()
        {
            _vm.User.Value = "Don Pedro";
            _vm.Password = null;
            Assert.IsTrue(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void OkCommandCanExecuteTrueForValidUsernameAndEmptyPassword()
        {
            _vm.User.Value = "Don Pedro";
            _vm.Password = "";
            Assert.IsTrue(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void ShouldCallOkHandler()
        {
            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User.Value = newCreds.Username;
            _vm.Password = newCreds.Password;

            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsFalse(result.UserCancelled);
                Assert.IsNotNull(result.Credentials);
                Assert.AreEqual(newCreds.Username, result.Credentials.Username);
                Assert.AreEqual(newCreds.Password, result.Credentials.Password);
                return null;
            });
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void ShouldCallCancelHandler()
        {
            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsTrue(result.UserCancelled);
                Assert.IsFalse(result.Save);
                Assert.IsNull(result.Credentials);
                return null;
            });
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        public void ModeShouldMatchArgsMode()
        {
            Assert.AreEqual(_args.Mode, _vm.Mode);            
        }

        [TestMethod]
        public void PasswordSetByArgs()
        {
            Assert.AreEqual(_args.Credentials.Password, _vm.Password);    
        }

        [TestMethod]
        public void UsernameSetByArgs()
        {
            Assert.AreEqual(_args.Credentials.Username, _vm.User.Value);            
        }

        [TestMethod]
        public void ShowSaveSetByArgs()
        {
            Assert.AreEqual(_args.ShowSave, _vm.ShowSave);            
        }

        [TestMethod]
        public void StoreCredentialsInitiallyFalse()
        {
            Assert.IsFalse(_vm.StoreCredentials);
        }

        [TestMethod]
        public void ShowOrHideMessageCorrectlyBasedOnMode()
        {
            List<CredentialPromptMode> showsMessage = new List<CredentialPromptMode>();
            showsMessage.Add(CredentialPromptMode.FreshCredentialsNeeded);
            showsMessage.Add(CredentialPromptMode.InvalidCredentials);

            foreach (CredentialPromptMode mode in Enum.GetValues(typeof(CredentialPromptMode)))
            {
                AddUserViewArgs args =
                    new AddUserViewArgs(
                        _testData.NewValidCredential().Model,
                        true,
                        mode);
                _vm = new AddOrEditUserViewModel();
                ((IViewModel)_vm).Presenting(_nav, args, _context);

                if (showsMessage.Any(m => mode.Equals(m)))
                {
                    Assert.IsTrue(_vm.ShowMessage);                    
                }
                else
                {
                    Assert.IsFalse(_vm.ShowMessage);
                }
            }
        }

        [TestMethod]
        public void CanDeleteTrueIffEditCredentialsMode()
        {
            foreach (CredentialPromptMode mode in Enum.GetValues(typeof(CredentialPromptMode)))
            {
                AddUserViewArgs args =
                    new AddUserViewArgs(
                        _testData.NewValidCredential().Model,
                        true,
                        mode);
                _vm = new AddOrEditUserViewModel();
                ((IViewModel)_vm).Presenting(_nav, args, _context);

                if (mode == CredentialPromptMode.EditCredentials)
                {
                    Assert.IsTrue(_vm.CanDelete);
                }
                else
                {
                    Assert.IsFalse(_vm.CanDelete);
                }
            }
        }

        [TestMethod]
        public void ShouldCallDeleteHandler()
        {
            _context.Expect("Dismiss", parameters =>
            {
                CredentialPromptResult result = parameters[0] as CredentialPromptResult;
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Deleted);
                return null;
            });
            _vm.Delete.Execute(null);
        }

        [TestMethod]
        public void CanDeleteTrueWhenEditingUser()
        {
            AddUserViewArgs args =
                new AddUserViewArgs(
                    _testData.NewValidCredential().Model,
                    true,
                    CredentialPromptMode.EditCredentials);
            ((IViewModel)_vm).Presenting(_nav, args, _context);
            Assert.IsTrue(_vm.CanDelete);
        }

        [TestMethod]
        public void CanDeleteFalseWhenAddingUser()
        {
            AddUserViewArgs args =
                new AddUserViewArgs(
                    _testData.NewValidCredential().Model,
                    true,
                    CredentialPromptMode.EnterCredentials);
            ((IViewModel)_vm).Presenting(_nav, args, _context);
            Assert.IsFalse(_vm.CanDelete);
        }
    }
}
