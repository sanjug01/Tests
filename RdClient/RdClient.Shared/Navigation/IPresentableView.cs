namespace RdClient.Navigation
{
    /// <summary>
    /// Interface of a UI object that may be presented by the view presenter component (IViewPresenter interface)
    /// </summary>
    public interface IPresentableView
    {
        void Activating(object activationParameter);
        void Presenting(INavigationService navigationService, object activationParameter);
        void Dismissing();
    }
}
