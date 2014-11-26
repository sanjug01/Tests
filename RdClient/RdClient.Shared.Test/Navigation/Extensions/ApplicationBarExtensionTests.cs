namespace RdClient.Shared.Test.Navigation.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

    [TestClass]
    public sealed class ApplicationBarExtensionTests
    {
        private TestAppBarViewModel _barVM;
        private ApplicationBarExtension _extension;
        private INavigationExtension _iExtension;

        [TestInitialize]
        public void SetUpTest()
        {
            _barVM = new TestAppBarViewModel();
            _extension = new ApplicationBarExtension();
            _iExtension = _extension;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _barVM = null;
            _extension.ViewModel = null;
            _extension = null;
            _iExtension = null;
        }

        [TestMethod]
        public void NewApplicationBarExtension_Clean()
        {
            Assert.IsNotNull(_extension);
            Assert.IsNotNull(_iExtension);
            Assert.IsNull(_extension.ViewModel);
        }

        [TestMethod]
        public void ChangeBarViewModel_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _extension.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _extension.ViewModel = _barVM;
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.IsTrue(reportedProperties.Contains("ViewModel"));
            Assert.AreSame(_barVM, _extension.ViewModel);
        }

        [TestMethod]
        public void PresentViewModelWithModels_ModelsSetToBar()
        {
            TestViewModel tvm = new TestViewModel() { Models = new BarItemModel[] { new SeparatorBarItemModel() } };

            _extension.ViewModel = _barVM;
            _iExtension.Presenting(tvm);
            Assert.AreSame(tvm.Models, _barVM.BarItems);
        }

        [TestMethod]
        public void DismissViewModelWithModels_BarCleared()
        {
            TestViewModel
                tvm1 = new TestViewModel() { Models = new BarItemModel[] { new SeparatorBarItemModel() } },
                tvm2 = new TestViewModel() { Models = new BarItemModel[] { new SeparatorBarItemModel() } };

            _extension.ViewModel = _barVM;
            _iExtension.Presenting(tvm1);
            _iExtension.Dismissed(tvm1);
            Assert.IsNull(_barVM.BarItems);
        }

        [TestMethod]
        public void PresentAnotherViewModelWithModels_ModelsSetToBar()
        {
            TestViewModel
                tvm1 = new TestViewModel() { Models = new BarItemModel[] { new SeparatorBarItemModel() } },
                tvm2 = new TestViewModel() { Models = new BarItemModel[] { new SeparatorBarItemModel() } };

            _extension.ViewModel = _barVM;
            _iExtension.Presenting(tvm1);
            _iExtension.Dismissed(tvm1);
            _iExtension.Presenting(tvm2);
            Assert.AreSame(tvm2.Models, _barVM.BarItems);
        }

        private sealed class TestAppBarViewModel : MutableObject, IApplicationBarViewModel
        {
            private readonly RelayCommand _doNothing;
            private IEnumerable<BarItemModel> _models;

            public TestAppBarViewModel()
            {
                _doNothing = new RelayCommand(o => { });
            }

            public IEnumerable<BarItemModel> BarItems
            {
                get { return _models; }
                set { this.SetProperty<IEnumerable<BarItemModel>>(ref _models, value ); }
            }

            bool IApplicationBarViewModel.IsBarAvailable
            {
                get { return null != _models; }
            }

            bool IApplicationBarViewModel.IsShowBarButtonVisible
            {
                get { return true; }
            }

            bool IApplicationBarViewModel.IsBarVisible
            {
                get { return true; }
                set { }
            }

            bool IApplicationBarViewModel.IsBarSticky
            {
                get { return false; }
                set { }
            }

            ICommand IApplicationBarViewModel.ShowBar
            {
                get { return _doNothing; }
            }
        }

        private sealed class TestViewModel : ViewModelBase, IApplicationBarItemsSource
        {
            public IApplicationBarSite BarSite;
            public IEnumerable<BarItemModel> Models;

            IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
            {
                this.BarSite = applicationBarSite;
                return this.Models;
            }
        }
    }
}
