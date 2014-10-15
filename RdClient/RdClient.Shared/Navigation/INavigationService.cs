namespace RdClient.Navigation
{
    /// <summary>
    /// Interface of a navigation service that navigates the UI between named views.
    /// The service supports two navigation modes - the main UI that is always shown, and a stack
    /// of modal views shown for a short time.
    /// </summary>
    public interface INavigationService
    {
        void NavigateToView(string viewName, object activationParameter);
        void PushModalView(string viewName, object activationParameter);
        void DismissModalView(IPresentableView modalView);
    }
}
