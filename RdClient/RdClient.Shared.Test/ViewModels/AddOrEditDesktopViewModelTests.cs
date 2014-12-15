using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
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
        private RdDataModel _dataModel;
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

            _dataModel = new RdDataModel();
            _addOrEditDesktopViewModel.DataModel = _dataModel;
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
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace);
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
        public void EditDesktop_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPc" };
                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
                Assert.IsFalse(_addOrEditDesktopViewModel.IsAddingDesktop);
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
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "foo" };
                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);
                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Add(desktop);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);

                Assert.AreEqual(1, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count);
                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0]);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveCredentials()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Credentials credentials = new Credentials() { Username = "foo", Password = "bar" };
                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Credentials.Add(credentials);

                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "foo" };
                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Add(desktop);

                _addOrEditDesktopViewModel.SelectedUserOptionsIndex = 2;

                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(desktop);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);

                Assert.AreEqual(1, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count);
                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0]);
                Assert.IsInstanceOfType(_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0], typeof(Desktop));
                Assert.AreEqual(credentials.Id, ((Desktop)_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0]).CredentialId);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSelectCorrectDefault()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Credentials credentials = new Credentials() { Username = "foo", Password = "bar" };

                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "foo", CredentialId = credentials.Id };
                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Add(desktop);

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
                Credentials credentials = new Credentials { Username = "Don Pedro", Password = "secret" };
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPc", CredentialId = credentials.Id };

                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Credentials.Add(credentials);
                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Add(desktop);

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
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPc" };

                _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Add(desktop);

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
                Desktop expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;

                Assert.AreEqual(0, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count);
                Assert.IsInstanceOfType(_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0], typeof(Desktop));
                Desktop savedDesktop = (Desktop)_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0];
                Assert.AreEqual(expectedDesktop.HostName, savedDesktop.HostName);
                Assert.AreNotEqual(expectedDesktop, savedDesktop, "A new desktop should have been created");
            }
        }

        [TestMethod]
        public void CancelAddDesktop_ShouldNotSaveNewDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                Desktop expectedDesktop = _testData.NewValidDesktop(Guid.Empty);

                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
                _addOrEditDesktopViewModel.CancelCommand.Execute(null);
                Assert.AreEqual(0, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count, "no desktop should be added when cancel command is executed");
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPC" };

                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);

                _addOrEditDesktopViewModel.Host = "myNewPC";
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);
                Assert.IsInstanceOfType(_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0], typeof(Desktop));
                Desktop addedDesktop = (Desktop)_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0];
                Assert.AreEqual(_addOrEditDesktopViewModel.Host, addedDesktop.HostName);
            }
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPC" };

                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);

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
                AddDesktopViewModelArgs args =
                    new AddDesktopViewModelArgs();

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                ((IViewModel)_addOrEditDesktopViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

                _addOrEditDesktopViewModel.Host = invalidHostName;

                Assert.AreEqual(0, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(0, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count, "Should not add desktop with invalid name!");
                Assert.IsFalse(_addOrEditDesktopViewModel.IsHostValid);

                // update name and save again
                _addOrEditDesktopViewModel.Host = validHostName;
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, _addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections.Count, "Should add desktop with valid name!");
                Assert.IsTrue(_addOrEditDesktopViewModel.IsHostValid);

                Desktop savedDesktop = (Desktop)_addOrEditDesktopViewModel.DataModel.LocalWorkspace.Connections[0];
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
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "myPC" };

                EditDesktopViewModelArgs args =
                    new EditDesktopViewModelArgs(desktop);

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
