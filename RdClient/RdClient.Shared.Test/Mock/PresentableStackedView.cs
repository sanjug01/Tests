namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Navigation;

    sealed class PresentableStackedView : PresentableView, IStackedView
    {
        void IStackedView.Activate()
        {
            Invoke(new object[] { });
        }

        void IStackedView.Deactivate()
        {
            Invoke(new object[] { });
        }
    }
}
