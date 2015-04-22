namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Base class for view view models of accessory views shown in the connection center.
    /// </summary>
    public abstract class AccessoryViewModelBase : ViewModelBase
    {

        protected void DismissSelfAndPushAccessoryView(string accessoryViewName, object dismissResult = null)
        {
            INavigationService nav = this.NavigationService;
            SynchronousCompletion can = _cancellation;

            DismissModal(dismissResult);
            nav.PushAccessoryView(accessoryViewName, can);
        }

    }
}
