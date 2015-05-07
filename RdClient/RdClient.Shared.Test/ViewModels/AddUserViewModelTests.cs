using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class AddUserViewModelTests
    {
        private TestData _testData;
        private Mock.ModalPresentationContext _context;
        private Mock.NavigationService _nav;
        private AddUserViewArgs _args;
        private AddUserViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _nav = new Mock.NavigationService();
            _context = new Mock.ModalPresentationContext();            
            _args = new AddUserViewArgs(_testData.NewValidCredential().Model, true, CredentialPromptMode.FreshCredentialsNeeded);
            _vm = new AddUserViewModel();
            ((IViewModel)_vm).Presenting(_nav, _args, _context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _nav.Dispose();
        }

        [TestMethod]
        public void AddUserViewModel_ShouldSetStoreCredentials()
        {
            _vm.StoreCredentials = true;
            Assert.IsTrue(_vm.StoreCredentials);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldUserNameValid()
        {
            _vm.User = "Don Pedro";
            Assert.IsTrue(_vm.IsUsernameValid);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldUserNameInvalid()
        {
            _vm.User = "Don Pedro>";
            Assert.IsFalse(_vm.IsUsernameValid);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteTrue()
        {
            _vm.User = "Don Pedro";
            _vm.Password = "secret";
            Assert.IsTrue(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse1()
        {
            _vm.User = null;
            _vm.Password = "secret";
            Assert.IsFalse(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse2()
        {
            _vm.User = "Don Pedro";
            _vm.Password = null;
            Assert.IsFalse(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse3()
        {
            _vm.User = "Don Pedro";
            _vm.Password = "";
            Assert.IsFalse(_vm.DefaultAction.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldCallOkHandler()
        {
            CredentialsModel newCreds = _testData.NewValidCredential().Model;
            _vm.User = newCreds.Username;
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
        public void AddUserViewModel_ShouldCallCancelHandler()
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
        public void AddUserViewModel_ModeShouldMatchArgsMode()
        {
            Assert.AreEqual(_args.Mode, _vm.Mode);            
        }

        [TestMethod]
        public void AddUserViewModel_PasswordSetByArgs()
        {
            Assert.AreEqual(_args.Credentials.Password, _vm.Password);    
        }

        [TestMethod]
        public void AddUserViewModel_UsernameSetByArgs()
        {
            Assert.AreEqual(_args.Credentials.Username, _vm.User);            
        }

        [TestMethod]
        public void AddUserViewModel_ShowSaveSetByArgs()
        {
            Assert.AreEqual(_args.ShowSave, _vm.ShowSave);            
        }

        [TestMethod]
        public void AddUserViewModel_StoreCredentialsInitiallyFalse()
        {
            Assert.IsFalse(_vm.StoreCredentials);
        }

        [TestMethod]
        public void AddUserViewModel_ShowMessageFalseIffInEnterCredentialsMode()
        {
            _vm.Mode = CredentialPromptMode.EnterCredentials;
            Assert.IsFalse(_vm.ShowMessage);
            foreach (CredentialPromptMode mode in Enum.GetValues(typeof(CredentialPromptMode)))
            {
                _vm.Mode = mode;
                if (mode == CredentialPromptMode.EnterCredentials)
                {
                    Assert.IsFalse(_vm.ShowMessage);
                }
                else
                {
                    Assert.IsTrue(_vm.ShowMessage);
                }
            }
        }

        [TestMethod]
        public void AddUserViewModel_EditUser_CanDelete()
        {
            AddUserViewArgs args =
                new AddUserViewArgs(
                    _testData.NewValidCredential().Model,
                    true,
                    CredentialPromptMode.EditCredentials);
            ((IViewModel)_vm).Presenting(_nav, args, _context);
            Assert.IsTrue(_vm.Delete.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_AddUser_CannotDelete()
        {
            AddUserViewArgs args = 
                new AddUserViewArgs(
                    _testData.NewValidCredential().Model,
                    true,
                    CredentialPromptMode.EnterCredentials);
            ((IViewModel)_vm).Presenting(_nav, args, _context);
            Assert.IsFalse(_vm.Delete.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldCallDeleteHandler()
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
    }
}
