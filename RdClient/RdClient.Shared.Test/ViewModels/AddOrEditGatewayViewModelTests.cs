﻿namespace RdClient.Shared.Test.ViewModels
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

    [TestClass]
    public class AddOrEditGatewayViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private TestAddOrEditGatewayViewModel _addOrEditGatewayVM;
        private GatewayModel _gateway;
        IModelContainer<GatewayModel> _gatewayContainer;
        private CredentialsModel _credentials;

        class TestAddOrEditGatewayViewModel : AddOrEditGatewayViewModel
        {
            public TestAddOrEditGatewayViewModel()
            {
                PresentableView = new Mock.PresentableView();
            }
        }

        [TestInitialize]
        public void TestSetUp()
        {
            _testData = new TestData();
            _addOrEditGatewayVM = new TestAddOrEditGatewayViewModel();
            _gateway = new GatewayModel() { HostName = _testData.NewRandomString() };
            _gatewayContainer = ModelContainer<GatewayModel>.CreateForNewModel(_gateway) ;
            _credentials = new CredentialsModel() { Username = _testData.NewRandomString(), Password = _testData.NewRandomString() };

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();
            ((IDataModelSite)_addOrEditGatewayVM).SetDataModel(_dataModel);
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _addOrEditGatewayVM = null;
        }

        [TestMethod]
        public void AddGateway_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                EditGatewayViewModelArgs args =
                    new EditGatewayViewModelArgs(_gatewayContainer);

                ((IViewModel) _addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(_gateway.HostName, _addOrEditGatewayVM.Host.Value);
            }
        }

        [TestMethod]
        public void AddGateway_CanSaveIfHostNameNotEmpty()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddGatewayViewModelArgs args =
                    new AddGatewayViewModelArgs();

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                _addOrEditGatewayVM.Host.Value = "MyPC";
                Assert.IsTrue(_addOrEditGatewayVM.DefaultAction.CanExecute(null));
                Assert.IsTrue(_addOrEditGatewayVM.Cancel.CanExecute(null));
            }
        }


        [TestMethod]
        public void EditGateway_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(_gateway.HostName, _addOrEditGatewayVM.Host.Value);
                Assert.IsFalse(_addOrEditGatewayVM.IsAddingGateway);
            }
        }

        [TestMethod]
        public void EditGateway_CannotSaveIfHostNameIsEmpty()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddGatewayViewModelArgs args =
                    new AddGatewayViewModelArgs();

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                _addOrEditGatewayVM.Host.Value = String.Empty;

                Assert.IsFalse(_addOrEditGatewayVM.DefaultAction.CanExecute(null));
                Assert.IsTrue(_addOrEditGatewayVM.Cancel.CanExecute(null));
            }
        }

        [TestMethod]
        public void EditGateway_ShouldNotAddExistingGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                _dataModel.Gateways.AddNewModel(_gateway);

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                _addOrEditGatewayVM.DefaultAction.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.AreEqual(_gateway, _dataModel.Gateways.Models[0].Model);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSaveCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Guid credId = _dataModel.Credentials.AddNewModel(_credentials);
                _dataModel.Gateways.AddNewModel(_gateway);
                _gatewayContainer = _dataModel.Gateways.Models[0];

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.SelectedUser = _addOrEditGatewayVM.Users[1];
                _addOrEditGatewayVM.DefaultAction.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                Assert.AreSame(_gateway, _dataModel.Gateways.Models[0].Model);
                Assert.AreEqual(credId, ((GatewayModel)_dataModel.Gateways.Models[0].Model).CredentialsId);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldResetCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Guid credId = _dataModel.Credentials.AddNewModel(_credentials);

                _gateway.CredentialsId = credId;
                _dataModel.Gateways.AddNewModel(_gateway);
                _gatewayContainer = _dataModel.Gateways.Models[0];

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.SelectedUser = _addOrEditGatewayVM.Users[0];
                _addOrEditGatewayVM.DefaultAction.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                Assert.AreSame(_gateway, _dataModel.Gateways.Models[0].Model);
                Assert.AreEqual(Guid.Empty, ((GatewayModel)_dataModel.Gateways.Models[0].Model).CredentialsId);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSelectReuseSession()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                _dataModel.Gateways.AddNewModel(_gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(UserComboBoxType.ReuseSession, _addOrEditGatewayVM.SelectedUser.UserComboBoxType);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSelectCorrectCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                _gateway.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);
                _dataModel.Gateways.AddNewModel(_gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(_credentials.Username, _addOrEditGatewayVM.SelectedUser.Credentials.Model.Username);
                Assert.AreSame(_credentials, _addOrEditGatewayVM.SelectedUser.Credentials.Model);
                Assert.AreEqual(_gateway.CredentialsId, _addOrEditGatewayVM.SelectedUser.Credentials.Id);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldUpdateSelectedItem()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                _dataModel.Gateways.AddNewModel(_gateway);
                _gateway.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(_credentials.Username, _addOrEditGatewayVM.SelectedUser.Credentials.Model.Username);
                Assert.AreEqual(_gateway.CredentialsId, _addOrEditGatewayVM.SelectedUser.Credentials.Id);
            }
        }

        [TestMethod]
        public void EditGateway_AddUser_ShouldOpenAddUserDialog()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                _dataModel.Gateways.AddNewModel(_gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                navigation.Expect("PushAccessoryView", new List<object> { "AddOrEditUserView", null, null }, null);

                // add User
                _addOrEditGatewayVM.AddUser.Execute(null);
            }
        }

        [TestMethod]
        public void AddGateway_ShouldSaveNewGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                GatewayModel expectedGateway = _testData.NewValidGatewayWithCredential(Guid.Empty);

                AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();

                _addOrEditGatewayVM.PresentableView = view;
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                _addOrEditGatewayVM.Host.Value = expectedGateway.HostName;

                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "no gateway should be added until save command is executed");
                _addOrEditGatewayVM.DefaultAction.Execute(null);
                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                GatewayModel savedGateway = (GatewayModel)_dataModel.Gateways.Models[0].Model;
                Assert.AreEqual(expectedGateway.HostName, savedGateway.HostName);
                Assert.AreNotSame(expectedGateway, savedGateway, "A new gateway should have been created");
            }
        }

        [TestMethod]
        public void CancelAddGateway_ShouldNotSaveNewGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                GatewayModel expectedGateway = _testData.NewValidGatewayWithCredential(Guid.Empty);

                AddGatewayViewModelArgs args =
                    new AddGatewayViewModelArgs();

                _addOrEditGatewayVM.PresentableView = view;

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host.Value = expectedGateway.HostName;
                _addOrEditGatewayVM.Cancel.Execute(null);
                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "no gateway should be added when cancel command is executed");
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSaveUpdatedGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();

                _dataModel.Gateways.AddNewModel(_gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);

                _addOrEditGatewayVM.PresentableView = view;

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host.Value = "myNewPC";
                _addOrEditGatewayVM.DefaultAction.Execute(saveParam);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                GatewayModel addedGateway = (GatewayModel)_dataModel.Gateways.Models[0].Model;
                Assert.AreEqual(_addOrEditGatewayVM.Host.Value, addedGateway.HostName);
            }
        }

        [TestMethod]
        public void CancelEditGateway_ShouldNotSaveUpdatedGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);

                _addOrEditGatewayVM.PresentableView = view;

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host.Value = "MyNewPC_not_updated";
                _addOrEditGatewayVM.Cancel.Execute(saveParam);

                Assert.AreNotEqual(_gateway.HostName, _addOrEditGatewayVM.Host);
            }
        }

        [TestMethod]
        public void AddGateway_SaveShouldValidateHostName()
        {
            string invalidHostName = "+MyPC";
            string validHostName = "MyPC";

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();

                _addOrEditGatewayVM.PresentableView = view;
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Empty);

                _addOrEditGatewayVM.Host.Value = invalidHostName;
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Invalid);

                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "no gateway should be added until save command is executed");
                _addOrEditGatewayVM.DefaultAction.Execute(null);
                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "Should not add gateway with invalid name!");

                // update name and save again
                _addOrEditGatewayVM.Host.Value = validHostName;
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Valid);

                _addOrEditGatewayVM.DefaultAction.Execute(null);
                Assert.AreEqual(1, _dataModel.Gateways.Models.Count, "Should add gateway with valid name!");

                GatewayModel savedGateway = (GatewayModel)_dataModel.Gateways.Models[0].Model;
                Assert.AreEqual(validHostName, savedGateway.HostName);
            }
        }

        [TestMethod]
        public void EditGateway_SaveShouldValidateHostName()
        {
            string invalidHostName = "MyNewPC+";
            string validHostName = "MyNewPC";

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);

                _addOrEditGatewayVM.PresentableView = view;                
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Valid);

                _addOrEditGatewayVM.Host.Value = invalidHostName;
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Invalid);

                _addOrEditGatewayVM.DefaultAction.Execute(saveParam);

                // update name and save again
                _addOrEditGatewayVM.Host.Value = validHostName;
                Assert.IsTrue(_addOrEditGatewayVM.Host.State.Status == ValidationResultStatus.Valid);
                _addOrEditGatewayVM.DefaultAction.Execute(saveParam);
            }
        }

        [TestMethod]
        public void AddGateway_AddUser_ShowsAddOrEditUserViewAndSelectsUser()
        {
            IPresentationCompletion completion = null;
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                Assert.AreEqual(0, _dataModel.Credentials.Models.Count);
                AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();

                _addOrEditGatewayVM.PresentableView = view;
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                navigation.Expect("PushAccessoryView", p =>
                {
                    Assert.AreEqual("AddOrEditUserView", p[0] as string);
                    var addUserArgs = p[1] as AddOrEditUserViewArgs;
                    Assert.AreEqual(CredentialPromptMode.EnterCredentials, addUserArgs.Mode);
                    completion = p[2] as IPresentationCompletion;
                    Assert.IsNotNull(completion);
                    return null;
                });
                _addOrEditGatewayVM.AddUser.Execute(null);

                //add user and call completion
                CredentialsModel credModel =  _testData.NewValidCredential().Model;
                Guid credId = _dataModel.Credentials.AddNewModel(credModel);
                IModelContainer<CredentialsModel> creds = TemporaryModelContainer<CredentialsModel>.WrapModel(credId, credModel);
                var promptResult = CredentialPromptResult.CreateWithCredentials(creds, true);
                completion.Completed(null, promptResult);

                // verify the new credentials are selected
                Assert.AreEqual(creds.Model, _addOrEditGatewayVM.SelectedUser.Credentials.Model);
            }
        }

        [TestMethod]
        public void AddGateway_CannotDelete()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddGatewayViewModelArgs args =
                    new AddGatewayViewModelArgs();
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                Assert.IsFalse(_addOrEditGatewayVM.Delete.CanExecute(null));
            }
        }

        [TestMethod]
        public void EditGateway_CanDelete()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                object saveParam = new object();
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_gatewayContainer);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);
                Assert.IsTrue(_addOrEditGatewayVM.Delete.CanExecute(null));
            }
        }

        [TestMethod]
        public void EditGateway_ShouldCallDeleteHandler()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.ModalPresentationContext context = new Mock.ModalPresentationContext())
            {
                _dataModel.Gateways.AddNewModel(_gateway);
                context.Expect("Dismiss", parameters =>
                {
                    GatewayPromptResult result = parameters[0] as GatewayPromptResult;
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result.Deleted);
                    return null;
                });
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(_dataModel.Gateways.Models[0]);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, context);
                _addOrEditGatewayVM.Delete.Execute(null);
            }           
        }

    }
}
