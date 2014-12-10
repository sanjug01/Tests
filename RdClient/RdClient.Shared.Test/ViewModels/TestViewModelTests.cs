using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    /// <summary>
    /// TestsViewModel model does not really need testing, 
    /// but missing test affect the code coverage.
    /// </summary>
    [TestClass]
    public class TestsViewModelTests
    {
        RdDataModel _dataModel;
        TestsViewModel _vm;
        TestsViewModelArgs _defaultArgs;

        [TestInitialize]
        public void TestSetUp()
        {
            _dataModel = new RdDataModel();
            _vm = new TestsViewModel();
            _defaultArgs = new TestsViewModelArgs(
                new Desktop(_dataModel.LocalWorkspace) { HostName = "DefaultPC" },
                new Credentials() { Username = "DefaultUser", Domain = "DefaultDomain", Password = "DefaultPwd" }
                );
            
            _vm.DataModel = _dataModel;
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _vm = null;
            _dataModel = null;
            _defaultArgs = null;
        }

        [TestMethod]
        public void TestsViewModel_VerifyPresenting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                TestsViewModelArgs args = new TestsViewModelArgs(
                    new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                    );

                ((IViewModel)_vm).Presenting(navigation, args, null);
            }
        }

        [TestMethod]
        public void TestsViewModel_VerifyTestDataNotEmpty()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                // test data is loaded only on presented
                Assert.IsTrue(_vm.Desktops.Count == 0);
                Assert.IsTrue(_vm.Users.Count == 0);

                ((IViewModel)_vm).Presenting(navigation, _defaultArgs, null);

                Assert.IsTrue(_vm.Desktops.Count > 0);
                Assert.IsTrue(_vm.Users.Count > 0);
            }
        }

        [TestMethod]
        public void TestsViewModel_VerifyTestDataCleanupOnDismiss()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                // test data is loaded only on presented
                ((IViewModel)_vm).Presenting(navigation, _defaultArgs, null);

                Assert.IsTrue(_vm.Desktops.Count > 0);
                Assert.IsTrue(_vm.Users.Count > 0);

                // test data is cleanup on dismissing
                ((IViewModel)_vm).Dismissing();
                Assert.IsTrue(_vm.Desktops.Count == 0);
                Assert.IsTrue(_vm.Users.Count == 0);
            }
        }

        [TestMethod]
        public void TestsViewModel_VerifyNoSelectedDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _defaultArgs, null);
                Assert.IsTrue(_vm.Desktops.Count > 0);
                // verify Edit, Delete cannnot execute
                Assert.IsFalse(_vm.EditDesktopCommand.CanExecute(null));
                Assert.IsFalse(_vm.DeleteDesktopCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void TestsViewModel_VerifySelectOneDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _defaultArgs, null);
                Assert.IsTrue(_vm.Desktops.Count > 0);

                IList<object> newSelection = new List<object>();
                newSelection.Add(_vm.Desktops[0]);

                _vm.SelectedDesktops = newSelection;

                // verify Edit, Delete can execute
                Assert.IsTrue(_vm.EditDesktopCommand.CanExecute(null));
                Assert.IsTrue(_vm.DeleteDesktopCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void TestsViewModel_VerifySelectMultipleDesktops()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _defaultArgs, null);
                Assert.IsTrue(_vm.Desktops.Count > 0);

                IList<object> newSelection = new List<object>();
                for (int i = 0; i < _vm.Desktops.Count; i = i + 2)
                {
                    newSelection.Add(_vm.Desktops[i]);
                }

                Assert.IsTrue(newSelection.Count > 0);
                _vm.SelectedDesktops = newSelection;

                // verify Edit cannot execute, Delete can execute
                Assert.IsFalse(_vm.EditDesktopCommand.CanExecute(null));
                Assert.IsTrue(_vm.DeleteDesktopCommand.CanExecute(null));
            }
        }

        [TestMethod]
        public void TestsViewModel_ShouldNavigateHome()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                TestsViewModelArgs args = new TestsViewModelArgs(
                    new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                    );

                navigation.Expect("NavigateToView", new List<object>() { "ConnectionCenterView", null }, 0);

                ((IViewModel)_vm).Presenting(navigation, args, null);
                _vm.GoHomeCommand.Execute(null);
            }
        }

        public void TestsViewModel_VerifyAppBarCommands()
        {
            // the appBar (and its commands) is not public for the view model
            // TODO : How to access the commands from appbar, other than declare them public?
            throw new NotImplementedException();
        }
    }
}
