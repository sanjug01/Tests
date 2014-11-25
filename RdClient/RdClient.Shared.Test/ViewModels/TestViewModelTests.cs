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

        [TestMethod]
        public void TestsViewModel_VerifyTestDataNotEmpty()
        {
            TestsViewModel vm = new TestsViewModel();

            Assert.IsTrue(vm.Desktops.Count > 0);
            Assert.IsTrue(vm.Users.Count > 0);
        }

        [TestMethod]
        public void TestsViewModel_VerifyNoSelectedDesktop()
        {
            TestsViewModel vm = new TestsViewModel();

            Assert.IsTrue(vm.Desktops.Count > 0);

            // verify Edit, Delete cannnot execute
            Assert.IsFalse(vm.EditDesktopCommand.CanExecute(null));
            Assert.IsFalse(vm.DeleteDesktopCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestsViewModel_VerifySelectOneDesktop()
        {
            TestsViewModel vm = new TestsViewModel();

            Assert.IsTrue(vm.Desktops.Count > 0);

            IList<object> newSelection = new List<object>();
            newSelection.Add(vm.Desktops[0]);

            vm.SelectedDesktops = newSelection;

            // verify Edit, Delete can execute
            Assert.IsTrue(vm.EditDesktopCommand.CanExecute(null));
            Assert.IsTrue(vm.DeleteDesktopCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestsViewModel_VerifySelectMultipleDesktops()
        {
            TestsViewModel vm = new TestsViewModel();

            Assert.IsTrue(vm.Desktops.Count > 0);

            IList<object> newSelection = new List<object>();
            for (int i = 0; i < vm.Desktops.Count; i = i + 2)
            {
                newSelection.Add(vm.Desktops[i]);
            }

            Assert.IsTrue(newSelection.Count > 0);
            vm.SelectedDesktops = newSelection;

            // verify Edit cannot execute, Delete can execute
            Assert.IsFalse(vm.EditDesktopCommand.CanExecute(null));
            Assert.IsTrue(vm.DeleteDesktopCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestsViewModel_VerifyPresenting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                TestsViewModel vm = new TestsViewModel();

                TestsViewModelArgs args = new TestsViewModelArgs(
                    new Desktop() { HostName = "narf" },
                    new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                    );

                ((IViewModel)vm).Presenting(navigation, args, null);
            }
        }

        [TestMethod]
        public void TestsViewModel_ShouldNavigateHome()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                TestsViewModel vm = new TestsViewModel();


                TestsViewModelArgs args = new TestsViewModelArgs(
                    new Desktop() { HostName = "narf" },
                    new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                    );

                navigation.Expect("NavigateToView", new List<object>() { "ConnectionCenterView", null }, 0);

                ((IViewModel)vm).Presenting(navigation, args, null);
                vm.GoHomeCommand.Execute(null);
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
