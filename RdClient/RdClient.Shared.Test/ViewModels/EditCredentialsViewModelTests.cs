namespace RdClient.Shared.Test.ViewModels
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
    using System.Collections.Generic;

    [TestClass]
    public sealed class EditCredentialsViewModelTests
    {
        private const string ViewName = "EditCredentialsView";

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

            IViewModel IPresentableView.ViewModel
            {
                get { return _vm; }
            }

            void IPresentableView.Activating(object activationParameter)
            {
            }

            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
            {
            }

            void IPresentableView.Dismissing()
            {
            }
        }

        private sealed class TestViewPresenter : IViewPresenter
        {

            void IViewPresenter.PresentView(IPresentableView view)
            {
            }

            void IViewPresenter.PushModalView(IPresentableView view)
            {
            }

            void IViewPresenter.DismissModalView(IPresentableView view)
            {
            }

            void IViewPresenter.PresentingFirstModalView()
            {
            }

            void IViewPresenter.DismissedLastModalView()
            {
            }
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

        private class ViewModelEventArgs : EventArgs
        {
            private readonly IEditCredentialsViewModel _viewModel;

            public ViewModelEventArgs(IEditCredentialsViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public IEditCredentialsViewModel ViewModel
            {
                get { return _viewModel; }
            }
        }

        private class ValidateEventArgs : ViewModelEventArgs
        {
            public ValidateEventArgs(IEditCredentialsViewModel viewModel, bool valid) : base(viewModel)
            {
                this.Valid = valid;
            }

            public bool Valid;
        }

        private class ValidatePropertyEventArgs : ValidateEventArgs
        {
            public ValidatePropertyEventArgs(IEditCredentialsViewModel viewModel, string propertyName, bool valid)
                : base(viewModel, valid)
            {
                this.PropertyName = propertyName;
            }

            public readonly string PropertyName;
        }

        private sealed class TestTask : EditCredentialsTaskBase
        {
            public event EventHandler<ViewModelEventArgs> Presenting;
            public event EventHandler<ViewModelEventArgs> Dismissed;
            public event EventHandler<ViewModelEventArgs> Cancelled;
            public event EventHandler<ValidateEventArgs> ValidateEvent;
            public event EventHandler<ValidatePropertyEventArgs> ValidatePropertyEvent;

            protected override void OnPresenting(IEditCredentialsViewModel viewModel)
            {
                if (null != this.Presenting)
                    this.Presenting(this, new ViewModelEventArgs(viewModel));
            }

            protected override bool Validate(IEditCredentialsViewModel viewModel)
            {
                ValidateEventArgs e = new ValidateEventArgs(viewModel, base.Validate(viewModel));

                if (null != this.ValidateEvent)
                    this.ValidateEvent(this, e);

                return e.Valid;
            }

            protected override bool ValidateChangedProperty(IEditCredentialsViewModel viewModel, string propertyName)
            {
                ValidatePropertyEventArgs e = new ValidatePropertyEventArgs(viewModel, propertyName, base.ValidateChangedProperty(viewModel, propertyName));

                if (null != this.ValidatePropertyEvent)
                    this.ValidatePropertyEvent(this, e);

                return e.Valid;
            }

            protected override void OnDismissing(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl viewControl)
            {
                base.OnDismissing(viewModel, viewControl);
            }

            protected override void OnDismissed(IEditCredentialsViewModel viewModel)
            {
                if (null != this.Dismissed)
                    this.Dismissed(this, new ViewModelEventArgs(viewModel));
            }

            protected override void OnCancelled(IEditCredentialsViewModel viewModel)
            {
                if (null != this.Cancelled)
                    this.Cancelled(this, new ViewModelEventArgs(viewModel));
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
            _nav = null;
            _vm = null;
            _dataModel = null;
        }

        [TestMethod]
        public void PushCredentialsEditor_TaskPresentedOnce()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();
            int validateCount = 0;

            task.Presenting += (sender, e) =>
            {
                Assert.AreSame(task, sender);
                presenting.Add(e);
            };
            task.Dismissed += (sender, e) => Assert.Fail();
            task.Cancelled += (sender, e) => Assert.Fail();
            task.ValidateEvent += (sender, e) =>
            {
                Assert.IsTrue(e.Valid);
                ++validateCount;
            };

            _nav.PushModalView(ViewName, task);

            Assert.AreEqual(1, presenting.Count);
            Assert.AreSame(_vm, presenting[0].ViewModel);
            Assert.AreEqual(1, validateCount);
            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void PushCredentialsEditor_FailValidation_CannotDismiss()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();

            task.Presenting += (sender, e) =>
            {
                Assert.AreSame(task, sender);
                presenting.Add(e);
            };
            task.Dismissed += (sender, e) => Assert.Fail();
            task.Cancelled += (sender, e) => Assert.Fail();
            task.ValidateEvent += (sender, e) => e.Valid = false;

            _nav.PushModalView(ViewName, task);

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangeUserName_ValidatesProperty()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();
            int validatePropertyCount = 0, validateCount = 0;

            _nav.PushModalView(ViewName, task);

            task.ValidatePropertyEvent += (sender, e) =>
            {
                Assert.AreEqual(EditCredentialsViewModel.UserNamePropertyName, e.PropertyName);
                Assert.IsTrue(e.Valid);
                //e.Valid = false;
                ++validatePropertyCount;
            };

            task.ValidateEvent += (sender, e) =>
            {
                Assert.AreEqual(1, validatePropertyCount);
                Assert.IsTrue(e.Valid);
                ++validateCount;
            };

            _vm.UserName = "pedro";

            Assert.AreEqual(1, validatePropertyCount);
            Assert.AreEqual(1, validateCount);
        }

        [TestMethod]
        public void CredentialsEditor_FailUserNameValidation_CannotDismiss()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();

            _nav.PushModalView(ViewName, task);

            task.ValidatePropertyEvent += (sender, e) => e.Valid = false;
            task.ValidateEvent += (sender, e) => Assert.Fail();

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.UserName = "pedro";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangeUserNameFailViewValidation_CannotDismiss()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();

            _nav.PushModalView(ViewName, task);

            task.ValidateEvent += (sender, e) => e.Valid = false;

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.UserName = "pedro";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_FailPasswordValidation_CannotDismiss()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();

            _nav.PushModalView(ViewName, task);

            task.ValidatePropertyEvent += (sender, e) => e.Valid = false;
            task.ValidateEvent += (sender, e) => Assert.Fail();

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Password = "password";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangePasswordFailViewValidation_CannotDismiss()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            TestTask task = new TestTask();

            _nav.PushModalView(ViewName, task);

            task.ValidateEvent += (sender, e) => e.Valid = false;

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Password = "password";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }
    }
}
