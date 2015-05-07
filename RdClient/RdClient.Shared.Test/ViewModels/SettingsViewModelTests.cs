namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class SettingsViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private Mock.ModalPresentationContext _context;
        private SettingsViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _navService = new Mock.NavigationService();
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                //
                // Set the data scrambler to use the local user's key
                //
                DataScrambler = new Rc4DataScrambler()
            };
            foreach (var cred in _testData.NewSmallListOfCredentials())
            {
                _dataModel.Credentials.AddNewModel(cred.Model);
            }
            foreach (var gateway in _testData.NewSmallListOfGateways())
            {
                _dataModel.Gateways.AddNewModel(gateway.Model);
            }

            _vm = new SettingsViewModel();
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            _context = new Mock.ModalPresentationContext();
            ((IViewModel)_vm).Presenting(_navService, null, null); 
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _context.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void TestCancelCommandDismisses()
        {
            ((IViewModel)_vm).Presenting(_navService, null, _context);
            _context.Expect("Dismiss", p => { return null; });
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        public void TestDefaultCommandDoesNothing()
        {
            ((IViewModel)_vm).Presenting(_navService, null, _context);
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void BackNavigationDismisses()
        {
            ((IViewModel)_vm).Presenting(_navService, null, _context);
            _context.Expect("Dismiss", p => { return null; });
            IBackCommandArgs backArgs = new BackCommandArgs();
            Assert.IsFalse(backArgs.Handled);
            (_vm as IViewModel).NavigatingBack(backArgs);
            Assert.IsTrue(backArgs.Handled);
        }

        [TestMethod]
        public void DismissingClearsThumbnailsIfUseThumbnailsIsSetToFalse()
        {
            ((IViewModel)_vm).Presenting(_navService, null, _context);
            _context.Expect("Dismiss", p => { return null; });
            foreach (var desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                desktop.Model.EncodedThumbnail = new byte[1];
            }
            _vm.GeneralSettings.UseThumbnails = false;
            _vm.Cancel.Execute(null);
            foreach (var desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                Assert.IsNull(desktop.Model.EncodedThumbnail);
            }
        }

        [TestMethod]
        public void DismissingDoesNotClearThumbnailsIfUseThumbnailsIsSetToTrue()
        {
            ((IViewModel)_vm).Presenting(_navService, null, _context);
            _context.Expect("Dismiss", p => { return null; });
            foreach (var desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                desktop.Model.EncodedThumbnail = new byte[1];
            }
            _vm.GeneralSettings.UseThumbnails = false;
            _vm.GeneralSettings.UseThumbnails = true;
            _vm.Cancel.Execute(null);
            foreach (var desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                Assert.IsNotNull(desktop.Model.EncodedThumbnail);
            }
        }

        [TestMethod]
        public void GeneralSettingsLoadedFromDataModel()
        {
            Assert.AreEqual(_vm.GeneralSettings, _dataModel.Settings);
        }

        [TestMethod]
        public void UsersLoadedFromDataModel()
        {
            AssertUserOptionsCorrect();
        }

        [TestMethod]
        public void GatewaysLoadedFromDataModel()
        {
            AssertGatewayOptionsCorrect();
        }

        [TestMethod]
        public void SelectingAddUserComboBoxItemShowsAddUserViewAddsUserToDatamodelAndUpdatesUsers()
        {
            IPresentationCompletion completion = null;

            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddUserView", p[0] as string);
                var args = p[1] as AddUserViewArgs;
                Assert.AreEqual(CredentialPromptMode.EnterCredentials, args.Mode);
                Assert.AreEqual("", args.Credentials.Username);
                Assert.AreEqual("", args.Credentials.Password);
                Assert.IsFalse(args.ShowSave);
                completion = p[2] as IPresentationCompletion;
                Assert.IsNotNull(completion);
                return null;
            });
            _vm.SelectedUser = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.AddNew);

            var newCreds = _testData.NewValidCredential().Model;
            var promptResult = CredentialPromptResult.CreateWithCredentials(newCreds, true);
            completion.Completed(null, promptResult);
            //creds were added
            Assert.IsTrue(_dataModel.Credentials.Models.Any(c => c.Model == newCreds));
            //users were updated
            AssertUserOptionsCorrect();
            Assert.IsNull(_vm.SelectedUser);
        }

        [TestMethod]
        public void EditUserCommandDisabledWhenSelectedUserIsNull()
        {
            _vm.SelectedUser = null;
            Assert.IsFalse(_vm.EditUserCommand.CanExecute(null));
        }

        [TestMethod]
        public void EditUserCommandEnabledWhenSelectedUserIsCredential()
        {
            _vm.SelectedUser = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            Assert.IsTrue(_vm.EditUserCommand.CanExecute(null));
        }

        [TestMethod]
        public void EditUserCommandShowsAddUserViewWithCorrectParameters()
        {
            IPresentationCompletion completion = null;
            var user = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            _vm.SelectedUser = user;

            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddUserView", p[0] as string);
                var args = p[1] as AddUserViewArgs;
                Assert.AreEqual(CredentialPromptMode.EditCredentials, args.Mode);
                Assert.AreEqual(_vm.SelectedUser.Credentials.Model, args.Credentials);
                Assert.IsFalse(args.ShowSave);
                completion = p[2] as IPresentationCompletion;
                Assert.IsNotNull(completion);
                return null;
            });
            _vm.EditUserCommand.Execute(null);

            user.Credentials.Model.Username = _testData.NewRandomString();
            var promptResult = CredentialPromptResult.CreateWithCredentials(user.Credentials.Model, true);
            completion.Completed(null, promptResult);
            AssertUserOptionsCorrect();
            Assert.IsNull(_vm.SelectedUser);
        }

        [TestMethod]
        public void DeleteUserCommandDisabledWhenSelectedUserIsNull()
        {
            _vm.SelectedUser = null;
            Assert.IsFalse(_vm.DeleteUserCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteUserCommandEnabledWhenSelectedUserIsCredential()
        {
            _vm.SelectedUser = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            Assert.IsTrue(_vm.DeleteUserCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteUserCommandCallsAddUserViewAndUpdatesUsersWhenComplete()
        {
            IPresentationCompletion completion = null;
            var user = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            _vm.SelectedUser = user;
            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("DeleteUserView", p[0] as string);
                Assert.AreEqual(user.Credentials, p[1]);
                completion = p[2] as IPresentationCompletion;
                return null;
            });
            _vm.DeleteUserCommand.Execute(null);
            _dataModel.Credentials.RemoveModel(user.Credentials.Id);           
            completion.Completed(null, null);
            AssertUserOptionsCorrect();
            Assert.IsNull(_vm.SelectedUser);
        }

        [TestMethod]
        public void SelectingAddGatewayComboBoxItemShowsExecutesAddGateway()
        {
            IPresentationCompletion completion = null;
            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddOrEditGatewayView", p[0] as string);
                Assert.IsTrue(p[1] is AddGatewayViewModelArgs);
                completion = p[2] as IPresentationCompletion;
                Assert.IsNotNull(completion);
                return null;
            });
            _vm.SelectedGateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.AddNew);

            Guid newCredId = _dataModel.Credentials.AddNewModel(_testData.NewValidCredential().Model);
            Guid newGatewayId = _dataModel.Gateways.AddNewModel(_testData.NewValidGatewayWithCredential(newCredId));
            var promptResult = GatewayPromptResult.CreateWithGateway(newGatewayId);
            completion.Completed(null, promptResult);
            
            AssertGatewayOptionsCorrect();
            Assert.IsNull(_vm.SelectedGateway);
            AssertUserOptionsCorrect();
        }

        [TestMethod]
        public void EditGatewayCommandDisabledWhenSelectedGatewayIsNull()
        {
            _vm.SelectedGateway = null;
            Assert.IsFalse(_vm.EditGatewayCommand.CanExecute(null));
        }

        [TestMethod]
        public void EditGatewayCommandEnabledWhenGatewayIsValid()
        {
            _vm.SelectedGateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway);
            Assert.IsTrue(_vm.EditGatewayCommand.CanExecute(null));
        }

        [TestMethod]
        public void EditGatewayCommandShowsAddOrEditGatewayViewWithCorrectParameters()
        {
            IPresentationCompletion completion = null;
            var gateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway);
            _vm.SelectedGateway = gateway;
            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddOrEditGatewayView", p[0] as string);
                var args = p[1] as EditGatewayViewModelArgs;
                Assert.AreEqual(gateway.Gateway.Model, args.Gateway);
                completion = p[2] as IPresentationCompletion;
                Assert.IsNotNull(completion);
                return null;
            });
            _vm.EditGatewayCommand.Execute(null);

            Guid newCredId = _dataModel.Credentials.AddNewModel(_testData.NewValidCredential().Model);
            gateway.Gateway.Model.CredentialsId = newCredId;
            gateway.Gateway.Model.HostName = _testData.NewRandomString();
            var promptResult = GatewayPromptResult.CreateWithGateway(Guid.Empty);
            completion.Completed(null, promptResult);

            AssertUserOptionsCorrect();
            AssertGatewayOptionsCorrect();
            Assert.IsNull(_vm.SelectedGateway);
        }

        [TestMethod]
        public void DeleteGatewayCommandDisabledWhenSelectedGateayIsNull()
        {
            _vm.SelectedGateway = null;
            Assert.IsFalse(_vm.DeleteGatewayCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteGatewayCommandEnabledWhenGatewayIsValid()
        {
            _vm.SelectedGateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway);
            Assert.IsTrue(_vm.DeleteGatewayCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteGatewayCommandCallsDeletGatewayViewAndUpdatesWhenComplete()
        {
            IPresentationCompletion completion = null;
            var gateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway);
            _vm.SelectedGateway = gateway;
            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("DeleteGatewayView", p[0] as string);
                Assert.AreEqual(gateway.Gateway, p[1]);
                completion = p[2] as IPresentationCompletion;
                Assert.IsNotNull(completion);
                return null;
            });
            _vm.DeleteGatewayCommand.Execute(null);
            _dataModel.Gateways.RemoveModel(gateway.Gateway.Id);
            completion.Completed(null, null);

            AssertUserOptionsCorrect();
            AssertGatewayOptionsCorrect();
            Assert.IsNull(_vm.SelectedGateway);
        }

        private void AssertUserOptionsCorrect()
        {
            //Add user option first
            Assert.AreEqual(UserComboBoxType.AddNew, _vm.Users[0].UserComboBoxType);
            //Remaining options match datamodel users
            Assert.AreEqual(_dataModel.Credentials.Models.Count + 1, _vm.Users.Count);
            var loadedUsers = _vm.Users.Where(u => u.UserComboBoxType == UserComboBoxType.Credentials).Select(u => u.Credentials).ToList();
            CollectionAssert.AreEqual(_dataModel.Credentials.Models, loadedUsers);
        }

        private void AssertGatewayOptionsCorrect()
        {
            //Add gateway option first
            Assert.AreEqual(GatewayComboBoxType.AddNew, _vm.Gateways[0].GatewayComboBoxType);
            //Remaining options match datamodel users
            Assert.AreEqual(_dataModel.Gateways.Models.Count + 1, _vm.Gateways.Count);
            var loadedGateways = _vm.Gateways.Where(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway).Select(g => g.Gateway).ToList();
            CollectionAssert.AreEqual(_dataModel.Gateways.Models, loadedGateways);
        }
    }
}
