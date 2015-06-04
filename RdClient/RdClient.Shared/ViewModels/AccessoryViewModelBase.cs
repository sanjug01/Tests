namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Windows.Input;


    /// <summary>
    /// Base class for view view models of accessory views shown in the connection center.
    /// </summary>
    public abstract class AccessoryViewModelBase : ViewModelBase, IDialogViewModel
    {
        private readonly ICommand _cancel;
        private readonly ICommand _defaultAction;

        protected AccessoryViewModelBase()
        {
            _cancel = new RelayCommand(parameter =>
            {
                if (!Cancel())
                    this.DismissModal(null);
            });

            _defaultAction = new RelayCommand(parameter => this.DefaultAction());
        }

        /// <summary>
        /// Overridables called when the Cancel command is executed.
        /// </summary>
        /// <returns>True if the overridable has fully processed cancellation and the view model
        /// must not dismiss itself. Otherwise, false.</returns>
        /// <remarks>Default implementation returns false and the view model dismisses itself
        /// with a null completion object.</remarks>
        protected virtual bool Cancel()
        {
            return false;
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

        protected void DismissSelfAndPushAccessoryView(string accessoryViewName, object activationParameter = null, object dismissResult = null)
        {
            INavigationService nav = this.NavigationService;

            DismissModal(dismissResult);
            nav.PushAccessoryView(accessoryViewName, activationParameter);
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            backArgs.Handled = true;
            //
            // Just dismiss self. This will pull the top-most accessory view from the stack.
            //
            DismissModal(null);
        }
    }
}
