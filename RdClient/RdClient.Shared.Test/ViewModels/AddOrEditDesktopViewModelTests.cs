using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class AddOrEditDesktopViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private TestAddOrEditDesktopViewModel _addOrEditDesktopViewModel;

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
                ModelSerializer = new SerializableModelSerializer()
            };
            ((IDataModelSite)_addOrEditDesktopViewModel).SetDataModel(_dataModel);
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _addOrEditDesktopViewModel = null;
        }

        [TestMethod]
        public void AddDesktop_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DesktopModel desktop = new DesktopModel();
                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);

                ((IViewModel) _addOrEditDesktopViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
            }
        }

        [TestMethod]
        public void AddDesktop_CanSaveIfHostNameNotEmpty()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = "MyPC";
                Assert.IsTrue(_addOrEditDesktopViewModel.SaveCommand.CanExecute(null));
                Assert.IsTrue(_addOrEditDesktopViewModel.CancelCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void AddDesktop_ShowHideExtraSettings()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

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
        }

        [TestMethod]
        public void EditDesktop_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DesktopModel desktop = new DesktopModel() { HostName = "myPc" };
                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
                Assert.IsFalse(_addOrEditDesktopViewModel.IsAddingDesktop);
            }
        }

        [TestMethod]
        public void EditDesktop_ShowHideExtraSettings()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DesktopModel desktop = new DesktopModel() { HostName = "myPc" };
                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

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
        }

        [TestMethod]
        public void EditDesktop_CannotSaveIfHostNameIsEmpty()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = String.Empty;

                Assert.IsFalse(_addOrEditDesktopViewModel.SaveCommand.CanExecute(null));
                Assert.IsTrue(_addOrEditDesktopViewModel.CancelCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldNotAddExistingDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DesktopModel desktop = new DesktopModel() { HostName = "foo" };
                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);

                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.AreEqual(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel() { Username = "foo", Password = "bar" };
                Guid credId = _dataModel.LocalWorkspace.Credentials.AddNewModel(credentials);

                DesktopModel desktop = new DesktopModel() { HostName = "foo" };
                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);                

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 2;

                navigation.Expect("DismissModalView", new List<object> { null }, null);
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);

                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                Assert.AreSame(desktop, _dataModel.LocalWorkspace.Connections.Models[0].Model);
                Assert.AreEqual(credId, ((DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model).CredentialsId);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectDefault()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Credentials credentials = new Credentials() { Username = "foo", Password = "bar" };

                DesktopModel desktop = new DesktopModel() { HostName = "foo", CredentialsId = credentials.Id };
                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(0, _addOrEditDesktopViewModel.SelectedUserOptionsIndex);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateSelectedIndex()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                CredentialsModel credentials = new CredentialsModel { Username = "Don Pedro", Password = "secret" };
                DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
                desktop.CredentialsId = _dataModel.LocalWorkspace.Credentials.AddNewModel(credentials);

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(2, _addOrEditDesktopViewModel.SelectedUserOptionsIndex);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldOpenAddUserDialog()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DesktopModel desktop = new DesktopModel() { HostName = "myPc" };

                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                navigation.Expect("PushModalView", new List<object> { "AddUserView", null, null }, null);

                _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 1;
            }
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;

                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
                Assert.AreEqual(expectedDesktop.HostName, savedDesktop.HostName);
                Assert.AreNotSame(expectedDesktop, savedDesktop, "A new desktop should have been created");
            }
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktopWithDefaultExtraSetting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;

                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;

                Assert.IsTrue(String.IsNullOrEmpty(savedDesktop.FriendlyName));
                Assert.AreEqual(false, savedDesktop.IsAdminSession);
                Assert.AreEqual(false, savedDesktop.IsSwapMouseButtons);
                Assert.AreEqual(AudioMode.Local, savedDesktop.AudioMode);
            }
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktopWithUpdatedExtraSetting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
                _addOrEditDesktopViewModel.FriendlyName = "FriendlyPc";
                _addOrEditDesktopViewModel.AudioMode = (int)Desktop.AudioModes.NoSound;
                _addOrEditDesktopViewModel.IsSwapMouseButtons = true;
                _addOrEditDesktopViewModel.IsUseAdminSession = true;

                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;

                Assert.AreEqual("FriendlyPc", savedDesktop.FriendlyName);
                Assert.AreEqual(true, savedDesktop.IsAdminSession);
                Assert.AreEqual(true, savedDesktop.IsSwapMouseButtons);
                Assert.AreEqual(AudioMode.NoSound, savedDesktop.AudioMode);
            }
        }

        [TestMethod]
        public void CancelAddDesktop_ShouldNotSaveNewDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DesktopModel expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
                _addOrEditDesktopViewModel.CancelCommand.Execute(null);
                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added when cancel command is executed");
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = "myNewPC";
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                DesktopModel addedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
                Assert.AreEqual(_addOrEditDesktopViewModel.Host, addedDesktop.HostName);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktopWithExtraSettings()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = "myNewPC";
                _addOrEditDesktopViewModel.FriendlyName = "FriendlyPc";
                _addOrEditDesktopViewModel.AudioMode = (int)Desktop.AudioModes.Remote;
                _addOrEditDesktopViewModel.IsSwapMouseButtons = true;
                _addOrEditDesktopViewModel.IsUseAdminSession = true;
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);

                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count);
                Assert.IsInstanceOfType(_dataModel.LocalWorkspace.Connections.Models[0].Model, typeof(DesktopModel));
                DesktopModel addedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
                Assert.AreEqual(_addOrEditDesktopViewModel.Host, addedDesktop.HostName);

                Assert.AreEqual("FriendlyPc", addedDesktop.FriendlyName);
                Assert.IsTrue(addedDesktop.IsAdminSession);
                Assert.IsTrue(addedDesktop.IsSwapMouseButtons);
                Assert.AreEqual(Desktop.AudioModes.Remote, addedDesktop.AudioMode);
            }
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = "MyNewPC_not_updated";
                _addOrEditDesktopViewModel.CancelCommand.Execute(saveParam);

                Assert.AreNotEqual(desktop.HostName, _addOrEditDesktopViewModel.Host);
            }
        }

        [TestMethod]
        public void AddDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "+MyPC";
            string validHostName = "MyPC";

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

                _addOrEditDesktopViewModel.Host = invalidHostName;

                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(0, _dataModel.LocalWorkspace.Connections.Models.Count, "Should not add desktop with invalid name!");
                Assert.IsFalse(_addOrEditDesktopViewModel.IsHostValid);

                // update name and save again
                _addOrEditDesktopViewModel.Host = validHostName;
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _dataModel.LocalWorkspace.Connections.Models.Count, "Should add desktop with valid name!");
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

                DesktopModel savedDesktop = (DesktopModel)_dataModel.LocalWorkspace.Connections.Models[0].Model;
                Assert.AreEqual(validHostName, savedDesktop.HostName);
            }
        }

        [TestMethod]
        public void EditDesktop_SaveShouldValidateHostName()
        {
            string invalidHostName = "MyNewPC+";
            string validHostName = "MyNewPC";

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                DesktopModel desktop = new DesktopModel() { HostName = "myPC" };

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = invalidHostName;
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);
                Assert.IsFalse(_addOrEditDesktopViewModel.IsHostValid);

                // update name and save again
                _addOrEditDesktopViewModel.Host = validHostName;
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);
            }
        }
    }
}
