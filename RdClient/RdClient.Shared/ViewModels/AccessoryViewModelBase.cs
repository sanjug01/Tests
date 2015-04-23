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

            DismissModal(dismissResult);
            nav.PushAccessoryView(accessoryViewName, null);
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            //
            // Just dismiss self. This will pull the top-most accessory view from the stack.
            //
            DismissModal(null);
        }
    }
}
