namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Navigation;

    sealed class PresentableViewWithStackedPresenter : PresentableView, IStackedViewPresenter
    {
        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            Invoke(new object[] { view, animated });
        }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Invoke(new object[] { view, animated });
        }
    }
}
