namespace RdClient.Shared.Test.ViewModels.EditCredentialsTasks
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Windows.Input;

    [TestClass]
    public sealed class InSessionCredentialsTaskTests
    {
        private static readonly string ViewName = "EditCredentialsView";

        private ApplicationDataModel _dataModel;
        private INavigationService _nav;
        private EditCredentialsViewModel _vm;

        private sealed class TestEditCredentialsView : IPresentableView
        {
            private readonly IViewModel _vm;

            public TestEditCredentialsView(IViewModel vm)
            {
                _vm = vm;
            }

            IViewModel IPresentableView.ViewModel { get { return _vm; } }
            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        private sealed class TestViewPresenter : IViewPresenter
        {

            void IViewPresenter.PresentView(IPresentableView view) { }
            void IViewPresenter.PushModalView(IPresentableView view) { }
            void IViewPresenter.DismissModalView(IPresentableView view) { }
            void IViewPresenter.PresentingFirstModalView() { }
            void IViewPresenter.DismissedLastModalView() { }
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
                Assert.IsInstanceOfType(activationParameter, typeof(IEditCredentialsTask));
                return new TestEditCredentialsView(_vm);
            }

            void IPresentableViewFactory.AddViewClass(string name, System.Type viewClass, bool isSingleton)
            {
                throw new System.NotImplementedException();
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _nav = new NavigationService();
            _vm = new EditCredentialsViewModel();
            _nav.ViewFactory = new TestViewFactory(_vm);
            _nav.Presenter = new TestViewPresenter();
            _nav.Extensions.Add(new DataModelExtension() { AppDataModel = _dataModel });
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
            _nav = null;
            _vm = null;
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_PresentNewCredentials_EmptyFields()
        {
            InSessionCredentialsTask task = new InSessionCredentialsTask(new SessionCredentials(), _dataModel);
            _nav.PushModalView(ViewName, task);

            Assert.IsTrue(string.IsNullOrEmpty(_vm.UserName));
            Assert.IsTrue(string.IsNullOrEmpty(_vm.Password));
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_PresentValidCredentials_CopiedFields()
        {
            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel);
            _nav.PushModalView(ViewName, task);

            Assert.AreEqual("don", _vm.UserName);
            Assert.AreEqual("pedro", _vm.Password);
            Assert.IsTrue(_vm.CanSaveCredentials);
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
        }
    }
}
