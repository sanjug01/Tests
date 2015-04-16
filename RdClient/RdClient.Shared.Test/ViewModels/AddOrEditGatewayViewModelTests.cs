namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class AddOrEditGatewayViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private TestAddOrEditGatewayViewModel _addOrEditGatewayVM;

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

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
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
                GatewayModel gateway = new GatewayModel();
                EditGatewayViewModelArgs args =
                    new EditGatewayViewModelArgs(gateway);

                ((IViewModel) _addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(gateway, _addOrEditGatewayVM.Gateway);
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
                _addOrEditGatewayVM.Host = "MyPC";
                Assert.IsTrue(_addOrEditGatewayVM.SaveCommand.CanExecute(null));
                Assert.IsTrue(_addOrEditGatewayVM.CancelCommand.CanExecute(null));
            }
        }


        [TestMethod]
        public void EditGateway_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                GatewayModel gateway = new GatewayModel() { HostName = "myPc" };
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(gateway, _addOrEditGatewayVM.Gateway);
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
                _addOrEditGatewayVM.Host = String.Empty;

                Assert.IsFalse(_addOrEditGatewayVM.SaveCommand.CanExecute(null));
                Assert.IsTrue(_addOrEditGatewayVM.CancelCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void EditGateway_ShouldNotAddExistingGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                GatewayModel gateway = new GatewayModel() { HostName = "foo" };
                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);

                _dataModel.Gateways.AddNewModel(gateway);

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.SaveCommand.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.AreEqual(gateway, _dataModel.Gateways.Models[0].Model);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSaveCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
                Guid credId = _dataModel.Credentials.AddNewModel(credentials);

                GatewayModel gateway = new GatewayModel() { HostName = "foo" };
                _dataModel.Gateways.AddNewModel(gateway); 

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.SelectedUserOptionsIndex = 2;
                _addOrEditGatewayVM.SaveCommand.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                Assert.AreSame(gateway, _dataModel.Gateways.Models[0].Model);
                Assert.AreEqual(credId, ((GatewayModel)_dataModel.Gateways.Models[0].Model).CredentialsId);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldResetCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
                Guid credId = _dataModel.Credentials.AddNewModel(credentials);

                GatewayModel gateway = new GatewayModel() { HostName = "foo", CredentialsId = credId };
                _dataModel.Gateways.AddNewModel(gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.SelectedUserOptionsIndex = 0;
                _addOrEditGatewayVM.SaveCommand.Execute(null);

                Assert.AreEqual(1, _dataModel.Gateways.Models.Count);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                Assert.AreSame(gateway, _dataModel.Gateways.Models[0].Model);
                Assert.AreEqual(Guid.Empty, ((GatewayModel)_dataModel.Gateways.Models[0].Model).CredentialsId);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSelectAskAlways()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };

                GatewayModel gateway = new GatewayModel()
                {
                    HostName = "foo"
                };
                _dataModel.Gateways.AddNewModel(gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(0, _addOrEditGatewayVM.SelectedUserOptionsIndex);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldSelectCorrectCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };

                GatewayModel gateway = new GatewayModel()
                {
                    HostName = "foo",
                    CredentialsId = _dataModel.Credentials.AddNewModel(credentials)
                };
                _dataModel.Gateways.AddNewModel(gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(2, _addOrEditGatewayVM.SelectedUserOptionsIndex);
                Assert.AreSame(credentials, _addOrEditGatewayVM.UserOptions[_addOrEditGatewayVM.SelectedUserOptionsIndex].Credentials.Model);
                Assert.AreEqual(gateway.CredentialsId, _addOrEditGatewayVM.UserOptions[_addOrEditGatewayVM.SelectedUserOptionsIndex].Credentials.Id);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldUpdateSelectedIndex()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel { Username = "Don Pedro", Password = "secret" };
                GatewayModel gateway = new GatewayModel() { HostName = "myPc" };

                _dataModel.Gateways.AddNewModel(gateway);
                gateway.CredentialsId = _dataModel.Credentials.AddNewModel(credentials);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                Assert.AreEqual(2, _addOrEditGatewayVM.SelectedUserOptionsIndex);
            }
        }

        [TestMethod]
        public void EditGateway_ShouldOpenAddUserDialog()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                GatewayModel gateway = new GatewayModel() { HostName = "myPc" };

                _dataModel.Gateways.AddNewModel(gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);
                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                navigation.Expect("PushModalView", new List<object> { "EditCredentialsView", null, null }, null);

                _addOrEditGatewayVM.SelectedUserOptionsIndex = 1;
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
                _addOrEditGatewayVM.Host = expectedGateway.HostName;

                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "no gateway should be added until save command is executed");
                _addOrEditGatewayVM.SaveCommand.Execute(null);
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

                _addOrEditGatewayVM.Host = expectedGateway.HostName;
                _addOrEditGatewayVM.CancelCommand.Execute(null);
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
                GatewayModel gateway = new GatewayModel() { HostName = "myPC" };

                _dataModel.Gateways.AddNewModel(gateway);

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);

                _addOrEditGatewayVM.PresentableView = view;

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host = "myNewPC";
                _addOrEditGatewayVM.SaveCommand.Execute(saveParam);
                Assert.IsInstanceOfType(_dataModel.Gateways.Models[0].Model, typeof(GatewayModel));
                GatewayModel addedGateway = (GatewayModel)_dataModel.Gateways.Models[0].Model;
                Assert.AreEqual(_addOrEditGatewayVM.Host, addedGateway.HostName);
            }
        }

        [TestMethod]
        public void CancelEditGateway_ShouldNotSaveUpdatedGateway()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                GatewayModel gateway = new GatewayModel() { HostName = "myPC" };

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);

                _addOrEditGatewayVM.PresentableView = view;

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host = "MyNewPC_not_updated";
                _addOrEditGatewayVM.CancelCommand.Execute(saveParam);

                Assert.AreNotEqual(gateway.HostName, _addOrEditGatewayVM.Host);
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
                Assert.IsTrue(_addOrEditGatewayVM.IsHostValid);

                _addOrEditGatewayVM.Host = invalidHostName;

                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "no gateway should be added until save command is executed");
                _addOrEditGatewayVM.SaveCommand.Execute(null);
                Assert.AreEqual(0, _dataModel.Gateways.Models.Count, "Should not add gateway with invalid name!");
                Assert.IsFalse(_addOrEditGatewayVM.IsHostValid);

                // update name and save again
                _addOrEditGatewayVM.Host = validHostName;
                _addOrEditGatewayVM.SaveCommand.Execute(null);
                Assert.AreEqual(1, _dataModel.Gateways.Models.Count, "Should add gateway with valid name!");
                Assert.IsTrue(_addOrEditGatewayVM.IsHostValid);

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
                GatewayModel gateway = new GatewayModel() { HostName = "myPC" };

                EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(gateway);

                _addOrEditGatewayVM.PresentableView = view;
                Assert.IsTrue(_addOrEditGatewayVM.IsHostValid);

                ((IViewModel)_addOrEditGatewayVM).Presenting(navigation, args, null);

                _addOrEditGatewayVM.Host = invalidHostName;
                _addOrEditGatewayVM.SaveCommand.Execute(saveParam);
                Assert.IsFalse(_addOrEditGatewayVM.IsHostValid);

                // update name and save again
                _addOrEditGatewayVM.Host = validHostName;
                _addOrEditGatewayVM.SaveCommand.Execute(saveParam);
                Assert.IsTrue(_addOrEditGatewayVM.IsHostValid);
            }
        }
    }
}
