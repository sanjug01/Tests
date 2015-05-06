namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class DesktopIdentityValidationCompletionTests
    {
        private const string ViewName = "DesktopIdentityValidationView";

        private ApplicationDataModel _dataModel;
        private INavigationService _nav;
        private DesktopIdentityValidationViewModel _vm;
        private DesktopModel _host;
        private DesktopIdentityValidationViewModelArgs _args;
        private TestValidation _validation;

        private sealed class TestView : IPresentableView
        {
            private readonly IViewModel _vm;

            public TestView(IViewModel vm) { _vm = vm; }
            IViewModel IPresentableView.ViewModel { get { return _vm; } }
            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        private sealed class TestViewPresenter : IViewPresenter, IStackedViewPresenter
        {
            void IViewPresenter.PresentView(IPresentableView view) { }
            void IStackedViewPresenter.PushView(IPresentableView view, bool animated) { }
            void IStackedViewPresenter.DismissView(IPresentableView view, bool animated) { }
        }

        private sealed class TestViewFactory : IPresentableViewFactory
        {
            private readonly IViewModel _vm;

            public TestViewFactory(IViewModel vm)
            {
                Assert.IsNotNull(vm);
                _vm = vm;
            }

            IPresentableView IPresentableViewFactory.CreateView(string name, object activationParameter)
            {
                Assert.AreEqual(ViewName, name);
                return new TestView(_vm);
            }

            void IPresentableViewFactory.AddViewClass(string name, System.Type viewClass, bool isSingleton) { }
        }

        private sealed class TestValidation : IServerIdentityValidation
        {
            private readonly DesktopModel _host;

            public event EventHandler Accepted;
            public event EventHandler Rejected;

            public TestValidation(DesktopModel host)
            {
                _host = host;
            }

            void IValidation.Accept()
            {
                if (null != this.Accepted)
                    this.Accepted(this, EventArgs.Empty);
            }

            void IValidation.Reject()
            {
                if (null != this.Rejected)
                    this.Rejected(this, EventArgs.Empty);
            }

            DesktopModel IServerIdentityValidation.Desktop
            {
                get { return _host; }
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                //
                // Set the data scrambler to use the local user's key
                //
                DataScrambler = new DataProtectionProviderDataScrambler() { Scope = "LOCAL=user" }
            };
            _nav = new NavigationService();
            _vm = new DesktopIdentityValidationViewModel();
            _nav.ViewFactory = new TestViewFactory(_vm);
            _nav.Presenter = new TestViewPresenter();
            _nav.Extensions.Add(new DataModelExtension() { AppDataModel = _dataModel });
            _host = new DesktopModel() { HostName = "MyHost" };
            _args = new DesktopIdentityValidationViewModelArgs("host");
            _validation = new TestValidation(_host);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vm = null;
            _nav = null;
            _dataModel = null;
            _host = null;
            _args = null;
            _validation = null;
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_AcceptOnce_AcceptedAndSessionTrusted()
        {
            DesktopIdentityValidationCompletion completion = 
                new DesktopIdentityValidationCompletion(_validation);
            int acceptedCount = 0;
            _validation.Accepted += (sender, e) => ++acceptedCount;
            _validation.Rejected += (sender, e) => Assert.Fail();

            _nav.PushModalView(ViewName, _args, completion);
            _vm.AcceptOnceCommand.Execute(null);

            Assert.IsFalse(_host.IsTrusted);
            Assert.AreEqual(1, acceptedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_AcceptAlways_AcceptedAndPermanentTrusted()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(_validation);
            int acceptedCount = 0;
            _validation.Accepted += (sender, e) => ++acceptedCount;
            _validation.Rejected += (sender, e) => Assert.Fail();

            _nav.PushModalView(ViewName, _args, completion);
            _vm.AcceptIdentityCommand.Execute(null);

            Assert.IsTrue(_host.IsTrusted);
            Assert.AreEqual(1, acceptedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_Reject_Rejected()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(_validation);
            int rejectedCount = 0;
            _validation.Accepted += (sender, e) => Assert.Fail();
            _validation.Rejected += (sender, e) => ++rejectedCount;

            _nav.PushModalView(ViewName, _args, completion);
            _vm.CancelCommand.Execute(null);

            Assert.IsFalse(_host.IsTrusted);
            Assert.AreEqual(1, rejectedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_NavigateBack_Rejected()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(_validation);
            int rejectedCount = 0;
            _validation.Accepted += (sender, e) => Assert.Fail();
            _validation.Rejected += (sender, e) => ++rejectedCount;

            _nav.PushModalView(ViewName, _args, completion);
            _vm.CastAndCall<IViewModel>(vm => vm.NavigatingBack(new BackCommandArgs()));

            Assert.IsFalse(_host.IsTrusted);
            Assert.AreEqual(1, rejectedCount);
        }
    }
}
