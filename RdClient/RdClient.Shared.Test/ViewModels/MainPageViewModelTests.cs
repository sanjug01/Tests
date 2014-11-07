namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    [TestClass]
    public class MainPageViewModelTests
    {
        private MainPageViewModel _vm;

        [TestInitialize]
        public void SetUpTest()
        {
            _vm = new MainPageViewModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vm = null;
        }

        [TestMethod]
        public void NewViewModel_CorrectApplicationBarProperties()
        {
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
            Assert.IsFalse(_vm.IsBarSticky);
        }

        [TestMethod]
        public void NewViewModel_ChangeIsShowBarButtonVisible_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.IsShowBarButtonVisible = true;
            Assert.IsTrue(_vm.IsShowBarButtonVisible);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "IsShowBarButtonVisible");
        }

        [TestMethod]
        public void NewViewModel_ChangeIsBarVisible_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.IsBarVisible = true;
            Assert.IsTrue(_vm.IsBarVisible);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "IsBarVisible");
        }

        [TestMethod]
        public void NewViewModel_ChangeIsBarSticky_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.IsBarSticky = true;
            Assert.IsTrue(_vm.IsBarSticky);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "IsBarSticky");
        }

        [TestMethod]
        public void NewViewModel_CannotExecuteShowBar()
        {
            Assert.IsFalse(_vm.ShowBar.CanExecute(null));
        }

        [TestMethod]
        public void NewViewModel_SetShowButtonVisible_CanExecuteShowBar()
        {
            _vm.IsShowBarButtonVisible = true;
            Assert.IsTrue(_vm.ShowBar.CanExecute(null));
        }

        [TestMethod]
        public void ButtonVisible_MakeBarVisible_ButtonHides()
        {
            _vm.IsShowBarButtonVisible = true;
            _vm.IsBarVisible = true;
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
        }

        [TestMethod]
        public void ButtonVisible_ExecuteShgowBar_BarShowsButtonHides()
        {
            _vm.IsShowBarButtonVisible = true;
            _vm.ShowBar.Execute(null);
            Assert.IsTrue(_vm.IsBarVisible);
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
        }
    }
}
