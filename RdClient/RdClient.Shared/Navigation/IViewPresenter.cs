namespace FadeTest.Navigation
{
    public interface IViewPresenter
    {
        void PresentView(IPresentableView view, INavigationService navigationService, object activationParameter);
        void PushModalView(IPresentableView view, INavigationService navigationService, object activationParameter);
    }
}
