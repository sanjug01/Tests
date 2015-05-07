namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [TestClass]
    public sealed class EditCredentialsViewModelTests
    {
        private const string ViewName = "EditCredentialsView";

        private ApplicationDataModel _dataModel;
        private INavigationService _nav;
        private EditCredentialsViewModel _vm;
        private TestTask _task;

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

        private sealed class DismissingEventArgs : ViewModelEventArgs
        {
            private readonly IEditCredentialsViewControl _viewControl;
            private bool _handled;

            public DismissingEventArgs(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl viewControl)
                : base(viewModel)
            {
                _viewControl = viewControl;
            }

            public void Dismiss()
            {
                _handled = true;
                _viewControl.Submit();
            }

            public void DoNotDismiss()
            {
                _handled = true;
            }

            public void AskConfirmation(EditCredentialsConfirmation message)
            {
                _handled = true;
                _viewControl.AskConfirmation(message);
            }

            public bool IsHandled
            {
                get { return _handled; }
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
            public event EventHandler<DismissingEventArgs> Dismissing;

            protected override void OnPresenting(IEditCredentialsViewModel viewModel)
            {
                base.OnPresenting(viewModel);
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
                DismissingEventArgs e = new DismissingEventArgs(viewModel, viewControl);

                if (null != this.Dismissing)
                    this.Dismissing(this, e);

                if(!e.IsHandled)
                    base.OnDismissing(viewModel, viewControl);
            }

            protected override void OnDismissed(IEditCredentialsViewModel viewModel)
            {
                base.OnDismissed(viewModel);
                if (null != this.Dismissed)
                    this.Dismissed(this, new ViewModelEventArgs(viewModel));
            }

            protected override void OnCancelled(IEditCredentialsViewModel viewModel)
            {
                base.OnCancelled(viewModel);
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
                ModelSerializer = new SerializableModelSerializer(),
                //
                // Set the data scrambler to use the local user's key
                //
                DataScrambler = new Rc4DataScrambler()
            };
            _nav = new NavigationService();
            _vm = new EditCredentialsViewModel();
            _nav.ViewFactory = new TestViewFactory(_vm);
            _nav.Presenter = new TestViewPresenter();
            _nav.Extensions.Add(new DataModelExtension() { AppDataModel = _dataModel });
            _task = new TestTask();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _task = null;
            _nav = null;
            _vm = null;
            _dataModel = null;
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeUserName_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.UserName = "user";
            _vm.UserName += "name";

            Assert.AreEqual("username", _vm.UserName);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(EditCredentialsViewModel.UserNamePropertyName, changes[0].PropertyName);
            Assert.AreEqual(EditCredentialsViewModel.UserNamePropertyName, changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangePassword_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.Password = "pass";
            _vm.Password += "word";

            Assert.AreEqual("password", _vm.Password);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(EditCredentialsViewModel.PasswordPropertyName, changes[0].PropertyName);
            Assert.AreEqual(EditCredentialsViewModel.PasswordPropertyName, changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeResourceName_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.ResourceName = "resource";
            _vm.ResourceName += "name";

            Assert.AreEqual("resourcename", _vm.ResourceName);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("ResourceName", changes[0].PropertyName);
            Assert.AreEqual("ResourceName", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangePrompt_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.Prompt = "pro";
            _vm.Prompt += "mpt";

            Assert.AreEqual("prompt", _vm.Prompt);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("Prompt", changes[0].PropertyName);
            Assert.AreEqual("Prompt", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeDismissLabel_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.DismissLabel = "dismiss";
            _vm.DismissLabel += "label";

            Assert.AreEqual("dismisslabel", _vm.DismissLabel);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("DismissLabel", changes[0].PropertyName);
            Assert.AreEqual("DismissLabel", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeSaveCredentials_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.SaveCredentials = !_vm.SaveCredentials;
            _vm.SaveCredentials = !_vm.SaveCredentials;

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("SaveCredentials", changes[0].PropertyName);
            Assert.AreEqual("SaveCredentials", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeCanSaveCredentials_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.CanSaveCredentials = !_vm.CanSaveCredentials;
            _vm.CanSaveCredentials = !_vm.CanSaveCredentials;

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("CanSaveCredentials", changes[0].PropertyName);
            Assert.AreEqual("CanSaveCredentials", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ChangeCanRevealPassword_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();

            _nav.PushModalView(ViewName, _task);
            _vm.PropertyChanged += (sender, e) => changes.Add(e);
            _vm.CanRevealPassword = !_vm.CanRevealPassword;
            _vm.CanRevealPassword = !_vm.CanRevealPassword;

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("CanRevealPassword", changes[0].PropertyName);
            Assert.AreEqual("CanRevealPassword", changes[1].PropertyName);
        }

        [TestMethod]
        public void EditCredentialsViewModel_ClearPassword_CanRevealPassword()
        {
            _nav.PushModalView(ViewName, _task);
            _vm.Password = "password";
            _vm.CanRevealPassword = false;
            _vm.Password = string.Empty;

            Assert.IsTrue(_vm.CanRevealPassword);
        }

        [TestMethod]
        public void EditCredentialsViewModel_NullClearPassword_CanRevealPassword()
        {
            _nav.PushModalView(ViewName, _task);
            _vm.Password = "password";
            _vm.CanRevealPassword = false;
            _vm.Password = null;

            Assert.IsTrue(_vm.CanRevealPassword);
        }

        [TestMethod]
        public void PushCredentialsEditor_TaskPresentedOnce()
        {
            IList<ViewModelEventArgs> presenting = new List<ViewModelEventArgs>();
            int validateCount = 0;

            _task.Presenting += (sender, e) =>
            {
                Assert.AreSame(_task, sender);
                presenting.Add(e);
            };
            _task.Dismissed += (sender, e) => Assert.Fail();
            _task.Cancelled += (sender, e) => Assert.Fail();
            _task.ValidateEvent += (sender, e) =>
            {
                Assert.IsTrue(e.Valid);
                ++validateCount;
            };

            _nav.PushModalView(ViewName, _task);

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

            _task.Presenting += (sender, e) =>
            {
                Assert.AreSame(_task, sender);
                presenting.Add(e);
            };
            _task.Dismissed += (sender, e) => Assert.Fail();
            _task.Cancelled += (sender, e) => Assert.Fail();
            _task.ValidateEvent += (sender, e) => e.Valid = false;

            _nav.PushModalView(ViewName, _task);

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
            Assert.IsTrue(_vm.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangeUserName_ValidatesProperty()
        {
            int validatePropertyCount = 0, validateCount = 0;

            _nav.PushModalView(ViewName, _task);

            _task.ValidatePropertyEvent += (sender, e) =>
            {
                Assert.AreEqual(EditCredentialsViewModel.UserNamePropertyName, e.PropertyName);
                Assert.IsTrue(e.Valid);
                ++validatePropertyCount;
            };

            _task.ValidateEvent += (sender, e) =>
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
            _nav.PushModalView(ViewName, _task);

            _task.ValidatePropertyEvent += (sender, e) => e.Valid = false;
            _task.ValidateEvent += (sender, e) => Assert.Fail();

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.UserName = "pedro";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangeUserNameFailViewValidation_CannotDismiss()
        {
            _nav.PushModalView(ViewName, _task);

            _task.ValidateEvent += (sender, e) => e.Valid = false;

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.UserName = "pedro";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_FailPasswordValidation_CannotDismiss()
        {
            _nav.PushModalView(ViewName, _task);

            _task.ValidatePropertyEvent += (sender, e) => e.Valid = false;
            _task.ValidateEvent += (sender, e) => Assert.Fail();

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Password = "password";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_ChangePasswordFailViewValidation_CannotDismiss()
        {
            _nav.PushModalView(ViewName, _task);

            _task.ValidateEvent += (sender, e) => e.Valid = false;

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Password = "password";

            Assert.IsFalse(_vm.Dismiss.CanExecute(null));
        }

        [TestMethod]
        public void CredentialsEditor_Dismiss_OnDismissingCalled()
        {
            int dismissingCount = 0;

            _nav.PushModalView(ViewName, _task);

            _task.Dismissing += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                e.DoNotDismiss();
                ++dismissingCount;
            };
            _task.Dismissed += (sender, e) => Assert.Fail();

            _vm.UserName = "user";
            _vm.Password = "password";

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Dismiss.Execute(null);

            Assert.AreEqual(1, dismissingCount);

        }

        [TestMethod]
        public void CredentialsEditor_DismissDismiss_Dismissed()
        {
            int dismissingCount = 0, dismissedCount = 0;

            _nav.PushModalView(ViewName, _task);

            _task.Dismissing += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                e.Dismiss();
                ++dismissingCount;
            };
            _task.Dismissed += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                ++dismissedCount;
            };

            _vm.UserName = "user";
            _vm.Password = "password";

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Dismiss.Execute(null);

            Assert.AreEqual(1, dismissingCount);
            Assert.AreEqual(1, dismissedCount);

        }

        [TestMethod]
        public void CredentialsEditor_DismissDoDefault_Dismissed()
        {
            int dismissingCount = 0, dismissedCount = 0;

            _nav.PushModalView(ViewName, _task);

            _task.Dismissing += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                ++dismissingCount;
            };
            _task.Dismissed += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                ++dismissedCount;
            };

            _vm.UserName = "user";
            _vm.Password = "password";

            Assert.IsTrue(_vm.Dismiss.CanExecute(null));
            _vm.Dismiss.Execute(null);

            Assert.AreEqual(1, dismissingCount);
            Assert.AreEqual(1, dismissedCount);

        }

        [TestMethod]
        public void CredentialsEditor_Cancel_Cancelled()
        {
            int cancelledCount = 0, dismissedViews = 0;

            _nav.PushModalView(ViewName, _task);
            _nav.DismissingLastModalView += (sender, e) => ++dismissedViews;

            _task.Dismissing += (sender, e) => Assert.Fail();
            _task.Dismissed += (sender, e) => Assert.Fail();
            _task.Cancelled += (sender, e) =>
            {
                Assert.AreSame(_vm, e.ViewModel);
                ++cancelledCount;
            };

            _vm.UserName = "user";
            _vm.Password = "password";

            Assert.IsTrue(_vm.Cancel.CanExecute(null));
            _vm.Cancel.Execute(null);

            Assert.AreEqual(1, cancelledCount);
            Assert.AreEqual(1, dismissedViews);
        }

        [TestMethod]
        public void CredentialsEditor_AskConfirmation_ConfirmationShown()
        {
            List<string> changedProperties = new List<string>();
            _task.Dismissing += (sender, e) => e.AskConfirmation(EditCredentialsConfirmation.OverridePassword);
            _task.ValidateEvent += (sender, e) => e.Valid = true;
            _task.Dismissed += (sender, e) => Assert.Fail();
            _task.Cancelled += (sender, e) => Assert.Fail();
            _vm.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

            Assert.IsFalse(_vm.IsConfirmationVisible);
            Assert.AreEqual(EditCredentialsConfirmation.NoMessage, _vm.ConfirmationMessage);
            _nav.PushModalView(ViewName, _task);
            _vm.Dismiss.Execute(null);

            CollectionAssert.Contains(changedProperties, "IsConfirmationVisible");
            CollectionAssert.Contains(changedProperties, "ConfirmationMessage");
            Assert.IsTrue(_vm.IsConfirmationVisible);
            Assert.AreEqual(EditCredentialsConfirmation.OverridePassword, _vm.ConfirmationMessage);
        }

        [TestMethod]
        public void CredentialsEditor_AskConfirmationConfirm_Submitted()
        {
            int dismissedCount = 0;

            _task.Dismissing += (sender, e) =>
            {
                e.AskConfirmation(EditCredentialsConfirmation.OverridePassword);
            };
            _task.ValidateEvent += (sender, e) => e.Valid = true;
            _task.Dismissed += (sender, e) => ++dismissedCount;
            _task.Cancelled += (sender, e) => Assert.Fail();

            Assert.IsFalse(_vm.IsConfirmationVisible);
            Assert.AreEqual(EditCredentialsConfirmation.NoMessage, _vm.ConfirmationMessage);
            _nav.PushModalView(ViewName, _task);
            _vm.Dismiss.Execute(null);
            Assert.IsTrue(_vm.Confirm.CanExecute(null));
            _vm.Confirm.Execute(null);

            Assert.IsFalse(_vm.IsConfirmationVisible);
            Assert.AreEqual(1, dismissedCount);
        }

        [TestMethod]
        public void CredentialsEditor_AskConfirmationCancel_NotSubmitted()
        {
            _task.Dismissing += (sender, e) =>
            {
                e.AskConfirmation(EditCredentialsConfirmation.OverridePassword);
            };
            _task.ValidateEvent += (sender, e) => e.Valid = true;
            _task.Dismissed += (sender, e) => Assert.Fail();
            _task.Cancelled += (sender, e) => Assert.Fail();

            Assert.IsFalse(_vm.IsConfirmationVisible);
            Assert.AreEqual(EditCredentialsConfirmation.NoMessage, _vm.ConfirmationMessage);
            _nav.PushModalView(ViewName, _task);
            _vm.Dismiss.Execute(null);
            Assert.IsTrue(_vm.Confirm.CanExecute(null));
            _vm.CancelConfirmation.Execute(null);

            Assert.IsFalse(_vm.IsConfirmationVisible);
        }
    }
}
