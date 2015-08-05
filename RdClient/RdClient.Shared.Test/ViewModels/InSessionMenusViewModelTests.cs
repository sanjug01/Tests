namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;
    using System;
    using System.Windows.Input;

    [TestClass]
    public sealed class InSessionMenusViewModelTests
    {
        private INavigationService _nav;
        private InSessionMenusViewModel _vm;
        private IViewModel _ivm;

        private sealed class MockModel : RdMock.MockBase, IInSessionMenus
        {
            private bool _invokeDispose;

            public readonly RelayCommand EnterFullScreenCommand;
            public readonly RelayCommand ExitFullScreenCommand;

            public MockModel(bool invokeDispose = false)
            {
                _invokeDispose = invokeDispose;
                this.EnterFullScreenCommand = new RelayCommand(p => { });
                this.ExitFullScreenCommand = new RelayCommand(p => { });
            }

            public override void Dispose()
            {
                if(_invokeDispose)
                    Invoke(new object[] { });
                base.Dispose();
            }

            void IInSessionMenus.Disconnect() { Invoke(new object[] { }); }

            ICommand IInSessionMenus.EnterFullScreen
            {
                get { return this.EnterFullScreenCommand; }
            }

            ICommand IInSessionMenus.ExitFullScreen
            {
                get { return this.ExitFullScreenCommand; }
            }
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
                Assert.IsNotNull(_vm.EnterFullScreen);
                Assert.AreSame(_vm.EnterFullScreen.Command, model.EnterFullScreenCommand);
                Assert.IsNotNull(_vm.ExitFullScreen);
                Assert.AreSame(_vm.ExitFullScreen.Command, model.ExitFullScreenCommand);
                Assert.AreSame(model.EnterFullScreenCommand, _vm.EnterFullScreen.Command);
                Assert.AreSame(model.ExitFullScreenCommand, _vm.ExitFullScreen.Command);
            }
        }

        [TestMethod]
        public void NewInSessionViewModel_PresentDismiss_ModelDisposed()
        {
            using (MockStackedContext context = new MockStackedContext())
            {
                MockModel model = new MockModel(true);
                model.Expect("Dispose", new List<object>() { }, null);

                _ivm.Presenting(_nav, model, context);
                _ivm.Dismissing();
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
