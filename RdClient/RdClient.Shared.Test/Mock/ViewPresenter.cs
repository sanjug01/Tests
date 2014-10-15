using RdClient.Navigation;

namespace Test.RdClient.Shared.Mock
{
    class ViewPresenter : IViewPresenter
    {
        private IPresentableView _presentedView;

        public ViewPresenter()
        {
        }

        public void PresentView(IPresentableView view)
        {
        }

        public void PushModalView(IPresentableView view)
        {
        }

        public void DismissModalView(IPresentableView view)
        {
        }
    }
}
