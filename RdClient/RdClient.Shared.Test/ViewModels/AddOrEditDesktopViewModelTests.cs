using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class AddOrEditDesktopViewModelTests
    {
        class TestAddOrEditDesktopViewModel : AddOrEditDesktopViewModel
        {
            public TestAddOrEditDesktopViewModel()
            {
                PresentableView = null;
            }
        }

        private TestAddOrEditDesktopViewModel _addOrEditDesktopViewModel;

        [TestInitialize]
        public void TestSetUp()
        {
            _addOrEditDesktopViewModel = new TestAddOrEditDesktopViewModel();
        }

        public void TestTearDown()
        {
            _addOrEditDesktopViewModel = null;
        }


        [TestMethod]
        public void AddDesktop_ShouldUseDefaultValues()
        {
            _addOrEditDesktopViewModel.Desktop = null;
            _addOrEditDesktopViewModel.IsAddingDesktop = true;

            Assert.IsTrue(string.IsNullOrEmpty(_addOrEditDesktopViewModel.Host));
        }

        [TestMethod]
        public void EditDesktop_ShouldUseDesktopValues()
        {
            Desktop desktop = new Desktop() { hostName = "myPc" };
            _addOrEditDesktopViewModel.IsAddingDesktop = false;
            _addOrEditDesktopViewModel.Desktop = desktop;

            Assert.AreEqual(desktop.hostName, _addOrEditDesktopViewModel.Host);
        }

        [TestMethod]
        public void AddDesktop_ShouldSaveNewDesktop()
        {
            object saveParam = new object();
            _addOrEditDesktopViewModel.Desktop = null;
            _addOrEditDesktopViewModel.IsAddingDesktop = true;

            _addOrEditDesktopViewModel.Host = "NewPC";
            _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);

            // TODO : verification that new desktop is persisted
        }

        [TestMethod]
        public void CancelAddDesktop_ShouldNotSaveNewDesktop()
        {
            object saveParam = new object();
            _addOrEditDesktopViewModel.Desktop = null;
            _addOrEditDesktopViewModel.IsAddingDesktop = true;

            _addOrEditDesktopViewModel.Host = "NewPC_not_saved";
            _addOrEditDesktopViewModel.CancelCommand.Execute(saveParam);

            Assert.IsTrue(string.IsNullOrEmpty(_addOrEditDesktopViewModel.Host));

            // TODO : verification that new desktop is not persisted
        }

        [TestMethod]
        public void EditDesktop_ShouldSaveUpdatedDesktop()
        {
            object saveParam = new object();
            Desktop desktop = new Desktop() { hostName = "myPC" };
            _addOrEditDesktopViewModel.IsAddingDesktop = false;

            _addOrEditDesktopViewModel.Host = "myNewPC";
            _addOrEditDesktopViewModel.SaveCommand.Execute(saveParam);

            // TODO : verification that new desktop is persisted
        }

        [TestMethod]
        public void CancelEditDesktop_ShouldNotSaveUpdatedDesktop()
        {
            object saveParam = new object();
            Desktop desktop = new Desktop() { hostName = "myPC" };
            _addOrEditDesktopViewModel.IsAddingDesktop = false;
            _addOrEditDesktopViewModel.Desktop = desktop;

            _addOrEditDesktopViewModel.Host = "MyNewPC_not_updated";
            _addOrEditDesktopViewModel.CancelCommand.Execute(saveParam);

            Assert.AreEqual(desktop.hostName, _addOrEditDesktopViewModel.Host);

            // TODO : verification that new desktop is not persisted
        }


    }
}
