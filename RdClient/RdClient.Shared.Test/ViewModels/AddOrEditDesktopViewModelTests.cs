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

    [TestClass]
    public class AddOrEditDesktopViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private TestAddOrEditDesktopViewModel _addOrEditDesktopViewModel;
        private Mock.NavigationService _nav;
        private Mock.PresentableView _view;

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
                //
                // Set the data scrambler to use the local user's key
                //
                DataScrambler = new Rc4DataScrambler()
            };
            ((IDataModelSite)_addOrEditDesktopViewModel).SetDataModel(_dataModel);

            _nav = new Mock.NavigationService();
            _view = new Mock.PresentableView();
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
            _addOrEditDesktopViewModel.Host = "MyPC";
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
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
            Assert.IsFalse(_addOrEditDesktopViewModel.IsAddingDesktop);        
        }

        [TestMethod]
        public void EditDesktop_ShowHideExtraSettings()
        {
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

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
            _addOrEditDesktopViewModel.Host = String.Empty;

            Assert.IsFalse(_addOrEditDesktopViewModel.DefaultAction.CanExecute(null));
            Assert.IsTrue(_addOrEditDesktopViewModel.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void EditDesktop_ShouldNotAddExistingDesktop()
        {
            DesktopModel desktop = new DesktopModel() { HostName = "foo" };
            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.AreEqual(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveCredentials()
        {
            CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
            Guid credId = _dataModel.Credentials.AddNewModel(credentials);

            DesktopModel desktop = new DesktopModel() { HostName = "foo" };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 2;

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(credId, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).CredentialsId);
        }

        [TestMethod]
        public void EditDesktop_ShouldResetCredentials()
        {
            CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
            Guid credId = _dataModel.Credentials.AddNewModel(credentials);

            DesktopModel desktop = new DesktopModel() { HostName = "foo", CredentialsId = credId };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 0;

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(Guid.Empty, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).CredentialsId);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectAskAlways()
        {
            CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };

            DesktopModel desktop = new DesktopModel()
            {
                HostName = "foo"
            };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(0, _addOrEditDesktopViewModel.SelectedUserOptionsIndex);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectCredentials()
        {
            CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };

            DesktopModel desktop = new DesktopModel()
            {
                HostName = "foo",
                CredentialsId = _dataModel.Credentials.AddNewModel(credentials)
            };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(2, _addOrEditDesktopViewModel.SelectedUserOptionsIndex);
            Assert.AreSame(credentials, _addOrEditDesktopViewModel.UserOptions[_addOrEditDesktopViewModel.SelectedUserOptionsIndex].Credentials.Model);
            Assert.AreEqual(desktop.CredentialsId, _addOrEditDesktopViewModel.UserOptions[_addOrEditDesktopViewModel.SelectedUserOptionsIndex].Credentials.Id);
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateSelectedIndex()
        {
            CredentialsModel credentials = new CredentialsModel { Username = "Don Pedro", Password = "secret" };
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
            desktop.CredentialsId = _dataModel.Credentials.AddNewModel(credentials);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(2, _addOrEditDesktopViewModel.SelectedUserOptionsIndex);
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenAddUserDialog()
        {
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddUserView", null, null }, null);

            _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 1;
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktop()
        {
            DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty).Model;

            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;

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
            _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;

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
            _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
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

            _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
            _addOrEditDesktopViewModel.Cancel.Execute(null);
            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added when cancel command is executed");
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            object saveParam = new object();
            DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host = "myNewPC";
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            DesktopModel addedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
            Assert.AreEqual(_addOrEditDesktopViewModel.Host, addedDesktop.HostName);
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktopWithExtraSettings()
        {
            object saveParam = new object();
            DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host = "myNewPC";
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

            Assert.AreEqual(_addOrEditDesktopViewModel.Host, desktop.HostName);
            Assert.AreEqual("FriendlyPc", desktop.FriendlyName);
            Assert.IsTrue(desktop.IsAdminSession);
            Assert.IsTrue(desktop.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.Remote, desktop.AudioMode);
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            object saveParam = new object();
            DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host = "MyNewPC_not_updated";
            _addOrEditDesktopViewModel.Cancel.Execute(saveParam);

            Assert.AreNotEqual(desktop.HostName, _addOrEditDesktopViewModel.Host);
        }

        [TestMethod]
        public void AddDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "+MyPC";
            string validHostName = "MyPC";

            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

            _addOrEditDesktopViewModel.PresentableView = _view;
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

            _addOrEditDesktopViewModel.Host = invalidHostName;

            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "Should not add desktop with invalid name!");
            Assert.IsFalse(_addOrEditDesktopViewModel.IsHostValid);

            // update name and save again
            _addOrEditDesktopViewModel.Host = validHostName;
            _addOrEditDesktopViewModel.DefaultAction.Execute(null);
            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count, "Should add desktop with valid name!");
            Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

            DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
            Assert.AreEqual(validHostName, savedDesktop.HostName);
        }

        [TestMethod]
        public void EditDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "MyNewPC+";
            string validHostName = "MyNewPC";

            object saveParam = new object();
            DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

            _addOrEditDesktopViewModel.PresentableView = _view;
            Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.Host = invalidHostName;
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);
            Assert.IsFalse(_addOrEditDesktopViewModel.IsHostValid);

            // update name and save again
            _addOrEditDesktopViewModel.Host = validHostName;
            _addOrEditDesktopViewModel.DefaultAction.Execute(saveParam);
            Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);
        }

        /* ****************************
        * *****  Gateway tests  *******
        ******************************* */
        [TestMethod]
        public void EditDesktop_ShouldSaveGateway()
        {
            GatewayModel gateway = new GatewayModel() { HostName = "fooGateway" };
            Guid gatewayId = _dataModel.Gateways.AddNewModel(gateway);

            DesktopModel desktop = new DesktopModel() { HostName = "foo" };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex = 2;

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(gatewayId, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).GatewayId);
        }

        [TestMethod]
        public void EditDesktop_ShouldResetGateway()
        {
            GatewayModel gateway = new GatewayModel() { HostName = "fooGateway" };
            Guid gatewayId = _dataModel.Gateways.AddNewModel(gateway);

            DesktopModel desktop = new DesktopModel() { HostName = "foo", GatewayId = gatewayId };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex = 0;

            _addOrEditDesktopViewModel.DefaultAction.Execute(null);

            Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
            Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
            Assert.AreSame(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            Assert.AreEqual(Guid.Empty, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).GatewayId);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectNoGatewayByDefault()
        {
            DesktopModel desktop = new DesktopModel()
            {
                HostName = "foo"
            };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(0, _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex);
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectGateway()
        {
            GatewayModel gateway = new GatewayModel() { HostName = "fooGateway" };

            DesktopModel desktop = new DesktopModel()
            {
                HostName = "foo",
                GatewayId = _dataModel.Gateways.AddNewModel(gateway)
            };
            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(2, _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex);
            Assert.AreSame(gateway, _addOrEditDesktopViewModel.GatewayOptions[_addOrEditDesktopViewModel.SelectedGatewayOptionsIndex].Gateway.Model);
            Assert.AreEqual(desktop.GatewayId, _addOrEditDesktopViewModel.GatewayOptions[_addOrEditDesktopViewModel.SelectedGatewayOptionsIndex].Gateway.Id);
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateSelectedGatewayIndex()
        {
            GatewayModel gateway = new GatewayModel() { HostName = "fooGateway" };
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
            desktop.GatewayId = _dataModel.Gateways.AddNewModel(gateway);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(2, _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex);
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenAddGatewayDialog()
        {
            DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

            _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

            EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
            ((IViewModel)_addOrEditDesktopViewModel).Presenting(_nav, args, null);

            _nav.Expect("PushAccessoryView", new List<object> { "AddOrEditGatewayView", null, null }, null);

            _addOrEditDesktopViewModel.SelectedGatewayOptionsIndex = 1;
        }
    }
}
