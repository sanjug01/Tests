using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
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

        class TestAddOrEditDesktopViewModel : AddOrEditDesktopViewModel
        {
            public TestAddOrEditDesktopViewModel()
            {
                PresentableView = new Mock.PresentableView();
            }
        }

        private TestAddOrEditDesktopViewModel _addOrEditDesktopViewModel;

        [TestInitialize]
        public void TestSetUp()
        {
            _testData = new TestData();
            _addOrEditDesktopViewModel = new TestAddOrEditDesktopViewModel();
        }
        
        [TestCleanup]
        public void TestTearDown()
        {
            _addOrEditDesktopViewModel = null;
        }


        [TestMethod]
        public void AddDesktop_ShouldUseDefaultValues()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = null;
                bool isAddingDesktop = true;
                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, null, isAddingDesktop);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                Assert.IsTrue(string.IsNullOrEmpty(_addOrEditDesktopViewModel.Host));
            }
        }

        [TestMethod]
        public void AddDesktop_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = null;
                Credentials creds = new Credentials { Username = "someuser", Password = "somepass", HaveBeenPersisted = false };
                bool isAddingDesktop = true;
                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, creds, isAddingDesktop);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
                Assert.AreEqual(creds, _addOrEditDesktopViewModel.Credentials);
                Assert.IsTrue(_addOrEditDesktopViewModel.IsAddingDesktop);
            }
        }

        [TestMethod]
        public void EditDesktop_PresentingShouldPassArgs()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = new Desktop() { HostName = "myPc" };
                Credentials creds = new Credentials { Username = "someuser", Password = "somepass", HaveBeenPersisted = false };
                bool isAddingDesktop = false;
                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, creds, isAddingDesktop);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                Assert.AreEqual(desktop, _addOrEditDesktopViewModel.Desktop);
                Assert.AreEqual(creds, _addOrEditDesktopViewModel.Credentials);
                Assert.IsFalse(_addOrEditDesktopViewModel.IsAddingDesktop);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldUseDesktopValues()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = new Desktop() { HostName = "myPc" };
                bool isAddingDesktop = false;

                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, null, isAddingDesktop);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                Assert.AreEqual(desktop.HostName, _addOrEditDesktopViewModel.Host);
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldUpdateCredentialsInComboBox()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Desktop desktop = new Desktop() { HostName = "myPc" };
                Credentials credentials = new Credentials { Username = "myUser", Domain = "MyDomain.com", Password = "MyPassword" };
                bool isAddingDesktop = false;

                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, credentials, isAddingDesktop);

                int initialCount = _addOrEditDesktopViewModel.UserOptions.Count;
                _addOrEditDesktopViewModel.Presenting(navigation, args);

                int finalCount = _addOrEditDesktopViewModel.UserOptions.Count;
                int userIndex = _addOrEditDesktopViewModel.UserOptions.IndexOf(credentials.Username);
                int selectedUSerIndex = _addOrEditDesktopViewModel.SelectedUserOptionsIndex;

                Assert.AreEqual(credentials, _addOrEditDesktopViewModel.Credentials);
                Assert.IsTrue(finalCount == initialCount + 1);
                Assert.IsTrue(userIndex >= 0);
                Assert.AreEqual(userIndex, selectedUSerIndex);
            }
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {                
                Desktop expectedDesktop = _testData.NewValidDesktop(Guid.Empty);
                dataModel.Desktops = new ModelCollection<Desktop>();
                _addOrEditDesktopViewModel.DataModel = dataModel;

                bool isAddingDesktop = true;
                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(null, null, isAddingDesktop);
                
                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                _addOrEditDesktopViewModel.Presenting(navigation, args);
                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;                
                
                Assert.AreEqual(0, dataModel.Desktops.Count, "no desktop should be added until save command is executed");
                _addOrEditDesktopViewModel.SaveCommand.Execute(null);
                Assert.AreEqual(1, dataModel.Desktops.Count);
                Desktop savedDesktop = dataModel.Desktops[0];
                Assert.AreEqual(expectedDesktop.HostName, savedDesktop.HostName);
                Assert.AreNotEqual(expectedDesktop, savedDesktop, "A new desktop should have been created");
            }
        }

        [TestMethod]
        public void CancelAddDesktop_ShouldNotSaveNewDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {
                Desktop expectedDesktop = _testData.NewValidDesktop(Guid.Empty);
                dataModel.Desktops = new ModelCollection<Desktop>();
                _addOrEditDesktopViewModel.DataModel = dataModel;
                
                bool isAddingDesktop = true;
                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(null, null, isAddingDesktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                _addOrEditDesktopViewModel.DataModel = dataModel;
                _addOrEditDesktopViewModel.Presenting(navigation, args);

                _addOrEditDesktopViewModel.Host = expectedDesktop.HostName;
                _addOrEditDesktopViewModel.CancelCommand.Execute(null);
                Assert.AreEqual(0, dataModel.Desktops.Count, "no desktop should be added when cancel command is executed");
            }
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {
                object saveParam = new object();
                Desktop desktop = new Desktop() { HostName = "myPC" };
                bool isAddingDesktop = false;

                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, null, isAddingDesktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                _addOrEditDesktopViewModel.Host = "myNewPC";
                _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);

            }
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                object saveParam = new object();
                Desktop desktop = new Desktop() { HostName = "myPC" };
                bool isAddingDesktop = false;

                AddOrEditDesktopViewModelArgs args =
                    new AddOrEditDesktopViewModelArgs(desktop, null, isAddingDesktop);

                _addOrEditDesktopViewModel.PresentableView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                _addOrEditDesktopViewModel.Presenting(navigation, args);

                _addOrEditDesktopViewModel.Host = "MyNewPC_not_updated";
                _addOrEditDesktopViewModel.CancelCommand.Execute(saveParam);

                Assert.AreEqual(desktop.HostName, _addOrEditDesktopViewModel.Host);
            }
        }
    }
}
