namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;


    /// <summary>
    /// Base class for view view models of accessory views shown in the connection center.
    /// </summary>
    public abstract class AccessoryViewModelBase : ViewModelBase, IDialogViewModel
    {
        private readonly ICommand _cancel;
        private readonly ICommand _defaultAction;
        private SynchronousCompletion _cancellation;

        protected SynchronousCompletion Cancellation { get { return _cancellation; } }

        protected AccessoryViewModelBase()
        {
            _cancel = new RelayCommand(parameter => this.DismissModal(null));
            _defaultAction = new RelayCommand(parameter => this.DefaultAction());
        }

        /// <summary>
        /// Overridable executed when user presses Enter in the accessory view. Default implementation does nothing.
        /// </summary>
        protected virtual void DefaultAction()
        {
        }

        ICommand IDialogViewModel.Cancel
        {
            get { return _cancel; }
        }

        ICommand IDialogViewModel.DefaultAction
        {
            get { return _defaultAction; }
        }

        protected void DismissSelfAndPushAccessoryView(string accessoryViewName, object dismissResult = null)
        {
            INavigationService nav = this.NavigationService;
            SynchronousCompletion can = _cancellation;

            DismissModal(dismissResult);
            nav.PushAccessoryView(accessoryViewName, can);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is SynchronousCompletion);
            base.OnPresenting(activationParameter);

            _cancellation = (SynchronousCompletion)activationParameter;
            _cancellation.Completed += this.OnCancellationRequested;
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _cancellation);

            base.OnDismissed();
            _cancellation.Completed -= this.OnCancellationRequested;
            _cancellation = null;
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            //
            // Just dismiss self. This will pull the top-most accessory view from the stack.
            //
            DismissModal(null);
        }

        private void OnCancellationRequested(object sender, EventArgs e)
        {
            Contract.Assert(object.ReferenceEquals(_cancellation, sender));
            this.DismissModal(null);
        }
    }
}
