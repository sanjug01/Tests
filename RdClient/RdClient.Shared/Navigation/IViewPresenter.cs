namespace RdClient.Navigation
{
    public interface IViewPresenter
    {
        void PresentView(IPresentableView view);
        void PushModalView(IPresentableView view);
        void DismissModalView(IPresentableView view);
    }
}
