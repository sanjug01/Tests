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

    [TestClass]
    public class AddOrEditDesktopViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private TestAddOrEditDesktopViewModel _addOrEditDesktopViewModel;
        private Mock.NavigationService _nav;
        private Mock.PresentableView _view;
        private CredentialsModel _credentials;
        private GatewayModel _gateway;
        private DesktopModel _desktop;

        class TestAddOrEditDesktopViewModel : AddOrEditDesktopViewModel
        {
            public TestAddOrEditDesktopViewModel()
            {
                PresentableView = new Mock.PresentableView();
            }
        }

        [TestInitialize]
        public void TestSetUp()
        {
            _testData = new TestData();
            _addOrEditDesktopViewModel = new TestAddOrEditDesktopViewModel();

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();
            ((IDataModelSite)_addOrEditDesktopViewModel).SetDataModel(_dataModel);

            _nav = new Mock.NavigationService();
            _view = new Mock.PresentableView();

            _credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
            _desktop = new DesktopModel()  { HostName = "foo" };
            _gateway = new GatewayModel() { HostName = "fooGateway" };
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _nav.Dispose();
            _view.Dispose();
        }

        [TestMethod]
        public void AddDesktop_PresentingShouldPassArgs()
        {
            DesktopModel desktop = new DesktopModel();
            EditDesktopViewModelArgs args =
                new EditDesktopViewModelArgs(desktop);

            ((IViewModel) _addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);            
        }

        [TestMethod]
        public void AddDesktop_CanSaveIfHostNameNotEmpty()
        {
            AddDesktopViewModelArgs args =
                new AddDesktopViewModelArgs();

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host.Value = "MyPC";
            Assert.IsTrue(_addOrEditDesktopViewModel.DefaultAction.CanExecute(null));
            Assert.IsTrue(_addOrEditDesktopViewModel.Cancel.CanExecute(null));            
        }

        [TestMethod]
        public void AddDesktop_ShowHideExtraSettings()
        {
            AddDesktopViewModelArgs args =
                new AddDesktopViewModelArgs();

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            // default hidden
            Assert.IsFalse(_addOrEditDesktopViewModel.IsExpandedView);

            // show 
            _addOrEditDesktopViewModel.ShowDetailsCommand.Execute(null);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsExpandedView);

            // hide again
            _addOrEditDesktopViewModel.HideDetailsCommand.Execute(null);
            Assert.IsFalse(_addOrEditDesktopViewModel.IsExpandedView);

            // show again
            _addOrEditDesktopViewModel.ShowDetailsCommand.Execute(null);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsExpandedView);
        }

        [TestMethod]
        public void EditDesktop_PresentingShouldPassArgs()
        {
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(_desktop, _addOrEditDesktopViewModel.Desktop);
            Assert.IsFalse(_addOrEditDesktopViewModel.IsAddingDesktop);        
        }

        [TestMethod]
        public void EditDesktop_ShowHideExtraSettings()
        {
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            // default hidden
            Assert.IsFalse(_addOrEditDesktopViewModel.IsExpandedView);

            // show 
            _addOrEditDesktopViewModel.ShowDetailsCommand.Execute(null);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsExpandedView);

            // hide again
            _addOrEditDesktopViewModel.HideDetailsCommand.Execute(null);
            Assert.IsFalse(_addOrEditDesktopViewModel.IsExpandedView);

            // show again
            _addOrEditDesktopViewModel.ShowDetailsCommand.Execute(null);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsExpandedView);
        }

        [TestMethod]
        public void EditDesktop_CannotSaveIfHostNameIsEmpty()
        {
            AddDesktopViewModelArgs args =
                new AddDesktopViewModelArgs();

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host.Value = String.Empty;

            Assert.IsFalse(_addOrEditDesktopViewModel.DefaultAction.CanExecute(null));
            Assert.IsTrue(_addOrEditDesktopViewModel.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_ShouldNotAddExistingDesktop()
        {
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.AreEqual(_desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveCredentials()
        {
            Guid credId = _dataModel.Credentials.AddNewModel(_credentials);            
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedUser = _addOrEditDesktopViewModel.Users[1];

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(_desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(credId, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).CredentialsId);
        }

        [TestMethod]
        public void EditDesktop_ShouldResetCredentials()
        {
            Guid credId = _dataModel.Credentials.AddNewModel(_credentials);
            _desktop.CredentialsId = credId;
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedUser = _addOrEditDesktopViewModel.Users[0];

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(_desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(Guid.Empty, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).CredentialsId);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectAskAlways()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(UserComboBoxType.AskEveryTime, _addOrEditDesktopViewModel.SelectedUser.UserComboBoxType);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectCredentials()
        {
            _desktop.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(_credentials.Username, _addOrEditDesktopViewModel.SelectedUser.Credentials.Model.Username);
            Assert.AreEqual(_desktop.CredentialsId, _addOrEditDesktopViewModel.SelectedUser.Credentials.Id);
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateSelectedItem()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(_credentials.Username, _addOrEditDesktopViewModel.SelectedUser.Credentials.Model.Username);
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenAddUserDialog()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddOrEditUserView", null, null }, null);

            _addOrEditDesktopViewModel.AddUser.Execute(null);
        }

        [TestMethod]
        public void EditDesktop_CannotEditDefaultUser()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.IsFalse(_addOrEditDesktopViewModel.EditUser.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_CanEditSelectedUser()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.IsTrue(_addOrEditDesktopViewModel.EditUser.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenEditUserDialog()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.CredentialsId = _dataModel.Credentials.AddNewModel(_credentials);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddOrEditUserView", null, null }, null);

            _addOrEditDesktopViewModel.EditUser.Execute(null);
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktop()
        {
            DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty).Model;

            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host.Value = expectedDesktop.HostName;

            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
            Assert.AreEqual(expectedDesktop.HostName, savedDesktop.HostName);
            Assert.AreNotSame(expectedDesktop, savedDesktop, "A new desktop should have been created");
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktopWithDefaultExtraSetting()
        {
            DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty).Model;

            AddDesktopViewModelArgs args =
                new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host.Value = expectedDesktop.HostName;

            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;

            Assert.IsTrue(String.IsNullOrEmpty(savedDesktop.FriendlyName));
            Assert.AreEqual(false, savedDesktop.IsAdminSession);
            Assert.AreEqual(false, savedDesktop.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.Local, savedDesktop.AudioMode);
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktopWithUpdatedExtraSetting()
        {
            DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty).Model;

            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host.Value = expectedDesktop.HostName;
            _addOrEditDesktopViewModel.FriendlyName = "FriendlyPc";
            _addOrEditDesktopViewModel.AudioMode = (int)AudioMode.NoSound;
            _addOrEditDesktopViewModel.IsSwapMouseButtons = true;
            _addOrEditDesktopViewModel.IsUseAdminSession = true;

            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;

            Assert.AreEqual("FriendlyPc", savedDesktop.FriendlyName);
            Assert.AreEqual(true, savedDesktop.IsAdminSession);
            Assert.AreEqual(true, savedDesktop.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.NoSound, savedDesktop.AudioMode);
        }

        [TestMethod]
        public void CancelAddDesktop_ShouldNotSaveNewDesktop()
        {
            DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty).Model;

            AddDesktopViewModelArgs args =
                new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = expectedDesktop.HostName;
            _addOrEditDesktopViewModel.Cancel.Execute(null);
            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added when cancel command is executed");
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            object saveParam = new object();

            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = "myNewPC";
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            DesktopModel addedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
            Assert.AreEqual(_addOrEditDesktopViewModel.Host.Value, addedDesktop.HostName);
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktopWithExtraSettings()
        {
            object saveParam = new object();

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = "myNewPC";
            _addOrEditDesktopViewModel.FriendlyName = "FriendlyPc";
            _addOrEditDesktopViewModel.AudioMode = (int)AudioMode.Remote;
            _addOrEditDesktopViewModel.IsSwapMouseButtons = true;
            _addOrEditDesktopViewModel.IsUseAdminSession = true;
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);

            //
            // Commented out - the _view model believes that an existing desktop is being edited,
            // so it doesn't add it to the data model.
            //
            //Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            //Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            //DesktopModel addedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;

            Assert.AreEqual(_addOrEditDesktopViewModel.Host.Value, _desktop.HostName);
            Assert.AreEqual("FriendlyPc", _desktop.FriendlyName);
            Assert.IsTrue(_desktop.IsAdminSession);
            Assert.IsTrue(_desktop.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.Remote, _desktop.AudioMode);
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            object saveParam = new object();

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = "MyNewPC_not_updated";
            _addOrEditDesktopViewModel.Cancel.Execute(saveParam);

            Assert.AreNotEqual(_desktop.HostName, _addOrEditDesktopViewModel.Host);
        }

        [TestMethod]
        public void AddDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "+MyPC";
            string validHostName = "MyPC";

            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = invalidHostName;
            Assert.IsTrue(_addOrEditDesktopViewModel.Host.State.Status == ValidationResultStatus.Invalid);

            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "Should not add desktop with invalid name!");
            

            // update name and save again
            _addOrEditDesktopViewModel.Host.Value = validHostName;
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count, "Should add desktop with valid name!");
            Assert.IsTrue(_addOrEditDesktopViewModel.Host.State.Status == ValidationResultStatus.Valid);

            DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
            Assert.AreEqual(validHostName, savedDesktop.HostName);
        }

        [TestMethod]
        public void EditDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "MyNewPC+";
            string validHostName = "MyNewPC";

            object saveParam = new object();

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host.Value = invalidHostName;
            Assert.IsTrue(_addOrEditDesktopViewModel.Host.State.Status == ValidationResultStatus.Invalid);
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);            

            // update name and save again
            _addOrEditDesktopViewModel.Host.Value = validHostName;
            Assert.IsTrue(_addOrEditDesktopViewModel.Host.State.Status == ValidationResultStatus.Valid);
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);            
        }

        /* ****************************
        * *****  Gateway tests  *******
        ******************************* */
        [TestMethod]
        public void EditDesktop_ShouldSaveGateway()
        {
            Guid gatewayId = _dataModel.Gateways.AddNewModel(_gateway);
            
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedGateway = _addOrEditDesktopViewModel.Gateways[1];

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(_desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(gatewayId, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).GatewayId);
        }

        [TestMethod]
        public void EditDesktop_ShouldResetGateway()
        {
            Guid gatewayId = _dataModel.Gateways.AddNewModel(_gateway);
            _desktop.GatewayId = gatewayId;
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedGateway = _addOrEditDesktopViewModel.Gateways[0];

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(_desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(Guid.Empty, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).GatewayId);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectNoGatewayByDefault()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(GatewayComboBoxType.None, _addOrEditDesktopViewModel.SelectedGateway.GatewayComboBoxType);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectGateway()
        {
            _desktop.GatewayId = _dataModel.Gateways.AddNewModel(_gateway);
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            
            Assert.AreSame(_gateway, _addOrEditDesktopViewModel.SelectedGateway.Gateway.Model);
            Assert.AreEqual(_desktop.GatewayId, _addOrEditDesktopViewModel.SelectedGateway.Gateway.Id);
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateSelectedGatewayIndex()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.GatewayId = _dataModel.Gateways.AddNewModel(_gateway);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(_gateway.HostName, _addOrEditDesktopViewModel.SelectedGateway.Gateway.Model.HostName);
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenAddGatewayDialog()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddOrEditGatewayView", null, null }, null);

            _addOrEditDesktopViewModel.AddGateway.Execute(null);
        }

        [TestMethod]
        public void EditDesktop_CannotEditDefaultGateway()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.IsFalse(_addOrEditDesktopViewModel.EditGateway.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_CanEditSelectedGateway()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.GatewayId = _dataModel.Gateways.AddNewModel(_gateway);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.IsTrue(_addOrEditDesktopViewModel.EditGateway.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenEditGatewayDialog()
        {
            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);
            _desktop.GatewayId = _dataModel.Gateways.AddNewModel(_gateway);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(_desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddOrEditGatewayView", null, null }, null);

            _addOrEditDesktopViewModel.EditGateway.Execute(null);
        }

    }
}
