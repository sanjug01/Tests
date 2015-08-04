namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    [TestClass]
    public sealed class InSessionMenusViewModelTests
    {
        private INavigationService _nav;
        private InSessionMenusViewModel _vm;
        private IViewModel _ivm;

        private sealed class MockModel : RdMock.MockBase, IInSessionMenus
        {
            void IInSessionMenus.Disconnect() { Invoke(new object[] { }); }
        }

        private sealed class MockStackedContext : RdMock.MockBase, IStackedPresentationContext
        {
            void IStackedPresentationContext.Dismiss(object result) { Invoke(new object[] { result }); }
        }

        [TestInitialize]
        public void TestSetup()
        {
            _nav = new NavigationService();
            _vm = new InSessionMenusViewModel();
            _ivm = _vm;
        }

        [TestCleanup]
        public void TestTeardown()
        {
            _vm = null;
            _ivm = null;
        }

        [TestMethod]
        public void NewInSessionViewModel_Present_CorrectSetup()
        {
            using (MockModel model = new MockModel())
            using (MockStackedContext context = new MockStackedContext())
            {
                _ivm.Presenting(_nav, model, context);
                Assert.IsNotNull(_vm.Cancel);
                Assert.IsNotNull(_vm.Disconnect);
                Assert.IsTrue(_vm.Cancel.CanExecute(null));
                Assert.IsTrue(_vm.Disconnect.CanExecute(null));
            }
        }

        [TestMethod]
        public void InSessionViewModel_Cancel_Dismissed()
        {
            using (MockModel model = new MockModel())
            using (MockStackedContext context = new MockStackedContext())
            {
                context.Expect("Dismiss", (parameters)=>
                {
                    Assert.IsNull(parameters[0]);
                    return null;
                });

                _ivm.Presenting(_nav, model, context);
                _vm.Cancel.Execute(null);
            }
        }

        [TestMethod]
        public void InSessionViewModel_Disconnect_CallsModel()
        {
            using (MockModel model = new MockModel())
            using (MockStackedContext context = new MockStackedContext())
            {
                model.Expect("Disconnect", new List<object> { }, null);

                _ivm.Presenting(_nav, model, context);
                _vm.Disconnect.Execute(null);

                Assert.IsFalse(_vm.Disconnect.CanExecute(null));
            }
        }
    }
}
