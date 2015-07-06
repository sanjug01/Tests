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
                DataScrambler = new Rc4DataScrambler()
            };
            _dataModel.Compose();
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
        public void AddUserShowsAddUserViewAndUpdatesUsers()
        {
            IPresentationCompletion completion = null;

            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddOrEditUserView", p[0] as string);
                var args = p[1] as AddOrEditUserViewArgs;
                Assert.AreEqual(CredentialPromptMode.EnterCredentials, args.Mode);
                completion = p[2] as IPresentationCompletion;
                return null;
            });

            _vm.AddUser.Execute(null);

            //simulate add user by AddOrEditUserView
            CredentialsModel credModel = _testData.NewValidCredential().Model;
            Guid credId = _dataModel.Credentials.AddNewModel(credModel);
            if (completion != null)//don't need this now, but may in the future
            {
                IModelContainer<CredentialsModel> newCreds = TemporaryModelContainer<CredentialsModel>.WrapModel(credId, credModel);
                var promptResult = CredentialPromptResult.CreateWithCredentials(newCreds, true);
                completion.Completed(null, promptResult);
            }

            //users were updated
            AssertUserOptionsCorrect();
            Assert.IsNull(_vm.SelectedUser);
        }

        [TestMethod]
        public void EditUserCommandDisabledWhenSelectedUserIsNull()
        {
            _vm.SelectedUser = null;
            Assert.IsFalse(_vm.EditUser.CanExecute(null));
        }

        [TestMethod]
        public void EditUserCommandEnabledWhenSelectedUserIsCredential()
        {
            _vm.SelectedUser = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            Assert.IsTrue(_vm.EditUser.CanExecute(null));
        }

        [TestMethod]
        public void EditUserCommandShowsAddUserViewWithCorrectParameters()
        {
            IPresentationCompletion completion = null;
            var user = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            _vm.SelectedUser = user;

            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddOrEditUserView", p[0] as string);
                var args = p[1] as AddOrEditUserViewArgs;
                Assert.AreEqual(CredentialPromptMode.EditCredentials, args.Mode);
                Assert.AreEqual(_vm.SelectedUser.Credentials.Model, args.Credentials.Model);
                Assert.IsFalse(args.ShowSave);
                completion = p[2] as IPresentationCompletion;
                return null;
            });
            _vm.EditUser.Execute(null);

            user.Credentials.Model.Username = _testData.NewRandomString();
            if (completion != null) //Not needed any more, but may be used again in the future
            {
                var promptResult = CredentialPromptResult.CreateWithCredentials(user.Credentials, true);
                completion.Completed(null, promptResult);
            }

            AssertUserOptionsCorrect(false);
            Assert.AreEqual(user, _vm.SelectedUser);
        }

        [TestMethod]
        public void UsersUpdatedCorrectlyOnDelete()
        {
            var user = _vm.Users.First(u => u.UserComboBoxType == UserComboBoxType.Credentials);
            _vm.SelectedUser = user;
            
            _dataModel.Credentials.RemoveModel(user.Credentials.Id);

            AssertUserOptionsCorrect();
            Assert.IsNull(_vm.SelectedUser);
        }


        [TestMethod]
        public void AddGatewayNavigatesToAddGatewayView()
        {
            _navService.Expect("PushAccessoryView", p =>
            {
                Assert.AreEqual("AddOrEditGatewayView", p[0] as string);
                Assert.IsTrue(p[1] is AddGatewayViewModelArgs);
                return null;
            });

            _vm.AddGateway.Execute(null);

            Guid newCredId = _dataModel.Credentials.AddNewModel(_testData.NewValidCredential().Model);
            Guid newGatewayId = _dataModel.Gateways.AddNewModel(_testData.NewValidGatewayWithCredential(newCredId));
            
            AssertGatewayOptionsCorrect();
            Assert.IsNull(_vm.SelectedGateway);
            AssertUserOptionsCorrect();
        }

        [TestMethod]
        public void EditGatewayCommandDisabledWhenSelectedGatewayIsNull()
        {
            _vm.SelectedGateway = null;
            Assert.IsFalse(_vm.EditGateway.CanExecute(null));
        }

        [TestMethod]
        public void EditGatewayCommandEnabledWhenGatewayIsValid()
        {
            _vm.SelectedGateway = _vm.Gateways.First(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway);
            Assert.IsTrue(_vm.EditGateway.CanExecute(null));
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
                Assert.AreEqual(gateway.Gateway, args.Gateway);
                return null;
            });
            _vm.EditGateway.Execute(null);
            
            Guid newCredId = _dataModel.Credentials.AddNewModel(_testData.NewValidCredential().Model);//EditGatewayView may add a user as well as a gateway
            gateway.Gateway.Model.CredentialsId = newCredId;
            gateway.Gateway.Model.HostName = _testData.NewRandomString();
            var promptResult = GatewayPromptResult.CreateWithGateway(Guid.Empty);
            completion.Completed(null, promptResult);

            AssertUserOptionsCorrect();
            // note that the order is broken for the edited element
            AssertGatewayOptionsCorrect(false);
            Assert.AreEqual(gateway, _vm.SelectedGateway);
        }

        private void AssertUserOptionsCorrect(bool verifyOrder = true)
        {
            //Add user option - no longer used
            //Remaining options match datamodel users
            Assert.AreEqual(_dataModel.Credentials.Models.Count, _vm.Users.Count);

            // users are ordered, verify collection equivalence
            var loadedUsers = _vm.Users.Where(u => u.UserComboBoxType == UserComboBoxType.Credentials).Select(u => u.Credentials).ToList();
            CollectionAssert.AreEquivalent(_dataModel.Credentials.Models, loadedUsers);

            // verify order
            if (verifyOrder)
            {
                for (int i = 0; i < loadedUsers.Count; i++)
                {
                    Assert.AreEqual(i, GetUserOrderInUsersCollection(loadedUsers[i].Model));
                }
            }
        }

        private void AssertGatewayOptionsCorrect(bool verifyOrder = true)
        {
            //Add gateway option - no longer used
            //Remaining options match datamodel users
            Assert.AreEqual(_dataModel.Gateways.Models.Count, _vm.Gateways.Count);

            // gateways are ordered, verify collection equivalence
            var loadedGateways = _vm.Gateways.Where(g => g.GatewayComboBoxType == GatewayComboBoxType.Gateway).Select(g => g.Gateway).ToList();
            CollectionAssert.AreEquivalent(_dataModel.Gateways.Models, loadedGateways);

            // verify order
            if (verifyOrder)
            {
                for (int i = 0; i < loadedGateways.Count; i++)
                {
                    Assert.AreEqual(i, GetGatewayOrderInGatewayCollection(loadedGateways[i].Model));
                }
            }
        }

        private int GetUserOrderInUsersCollection(CredentialsModel model)
        {
            int position = 0;
            for(int i = 0; i < _dataModel.Credentials.Models.Count; i++)
            {
                // ordered by username
                if(0 < model.Username.CompareTo(_dataModel.Credentials.Models[i].Model.Username))
                {
                    position++;
                }
            }

            return position;
        }

        private int GetGatewayOrderInGatewayCollection(GatewayModel model)
        {
            int position = 0;
            for (int i = 0; i < _dataModel.Gateways.Models.Count; i++)
            {
                // ordered by hostname
                if (0 < model.HostName.CompareTo(_dataModel.Gateways.Models[i].Model.HostName))
                {
                    position++;
                }
            }

            return position;
        }
    }
}
