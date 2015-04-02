namespace RdClient.Shared.Test.ViewModels.EditCredentialsTasks
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System.Collections.Generic;

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
            InSessionCredentialsTask task = new InSessionCredentialsTask(new SessionCredentials(), _dataModel, "Prompt", null);
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
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);

            Assert.AreEqual("don", _vm.UserName);
            Assert.AreEqual("pedro", _vm.Password);
            Assert.IsTrue(_vm.CanSaveCredentials);
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUser_PasswordCopied()
        {
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username="peter", Password="rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";

            Assert.AreEqual("rabbit", _vm.Password);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUserChangeUser_PasswordCleared()
        {
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "peter", Password = "rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";
            _vm.UserName += "2";

            Assert.IsTrue(string.IsNullOrEmpty(_vm.Password));
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_PresentNewTypeSubmit_TypedValues()
        {
            SessionCredentials sc = new SessionCredentials();
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "don";
            _vm.Password = "pedro";
            _vm.Dismiss.Execute(null);

            Assert.AreEqual("don", sc.Credentials.Username);
            Assert.AreEqual("pedro", sc.Credentials.Password);
            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_Submit_StatePassed()
        {
            object state = new object();
            int submitCount = 0;
            SessionCredentials sc = new SessionCredentials();
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", state);
            task.Submitted += (sender, e) =>
            {
                ++submitCount;
                Assert.AreSame(state, e.State);
            };
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "don";
            _vm.Password = "pedro";
            _vm.Dismiss.Execute(null);

            Assert.AreEqual(1, submitCount);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUserSubmit_Submitted()
        {
            IList<InSessionCredentialsTask.SubmittedEventArgs> submitted = new List<InSessionCredentialsTask.SubmittedEventArgs>();
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "peter", Password = "rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";
            task.Submitted += (sender, e) => submitted.Add(e);
            task.Cancelled += (sender, e) => Assert.Fail();
            _vm.Dismiss.Execute(null);

            Assert.AreEqual(1, submitted.Count);
            Assert.IsFalse(submitted[0].SaveCredentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual("peter", sc.Credentials.Username);
            Assert.AreEqual("rabbit", sc.Credentials.Password);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUserCheckSaveSubmit_Submitted()
        {
            IList<InSessionCredentialsTask.SubmittedEventArgs> submitted = new List<InSessionCredentialsTask.SubmittedEventArgs>();
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "peter", Password = "rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";
            _vm.SaveCredentials = true;
            task.Submitted += (sender, e) => submitted.Add(e);
            task.Cancelled += (sender, e) => Assert.Fail();
            _vm.Dismiss.Execute(null);

            Assert.IsFalse(_vm.IsConfirmationVisible);
            Assert.AreEqual(1, submitted.Count);
            Assert.IsTrue(submitted[0].SaveCredentials);
            Assert.AreEqual("peter", sc.Credentials.Username);
            Assert.AreEqual("rabbit", sc.Credentials.Password);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUserChangePasswordSubmit_Confirm()
        {
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "peter", Password = "rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";
            _vm.Password = "joppa";
            _vm.SaveCredentials = true;
            task.Submitted += (sender, e) => Assert.Fail();
            task.Cancelled += (sender, e) => Assert.Fail();
            _vm.Dismiss.Execute(null);

            Assert.IsTrue(_vm.IsConfirmationVisible);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeExistingUserChangePasswordConfirm_Submitted()
        {
            IList<InSessionCredentialsTask.SubmittedEventArgs> submitted = new List<InSessionCredentialsTask.SubmittedEventArgs>();
            _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "peter", Password = "rabbit" });

            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "peter";
            _vm.Password = "joppa";
            _vm.SaveCredentials = true;
            task.Submitted += (sender, e) => submitted.Add(e);
            task.Cancelled += (sender, e) => Assert.Fail();
            _vm.Dismiss.Execute(null);
            _vm.Confirm.Execute(null);

            Assert.AreEqual(1, submitted.Count);
            Assert.IsTrue(submitted[0].SaveCredentials);
            Assert.AreEqual("peter", sc.Credentials.Username);
            Assert.AreEqual("joppa", sc.Credentials.Password);
            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_TypeCancel_NotChanged()
        {
            int cancelledCount = 0;
            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", null);
            task.Cancelled += (sender, e) => ++cancelledCount;
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "doctor";
            _vm.Password = "hyder";
            _vm.Cancel.Execute(null);

            Assert.AreEqual(1, cancelledCount);
            Assert.AreEqual("don", sc.Credentials.Username);
            Assert.AreEqual("pedro", sc.Credentials.Password);
            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void InSessionEditCredentialsTask_Cancel_StatePassed()
        {
            int cancelledCount = 0;
            object state = new object();
            SessionCredentials sc = new SessionCredentials();
            sc.Credentials.Username = "don";
            sc.Credentials.Password = "pedro";
            InSessionCredentialsTask task = new InSessionCredentialsTask(sc, _dataModel, "Prompt", state);
            task.Cancelled += (sender, e) =>
            {
                ++cancelledCount;
                Assert.AreSame(state, e.State);
            };
            _nav.PushModalView(ViewName, task);
            _vm.UserName = "doctor";
            _vm.Password = "hyder";
            _vm.Cancel.Execute(null);

            Assert.AreEqual(1, cancelledCount);
        }
    }
}
