using RdClient.Shared.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class ViewPresenter : MockBase, IViewPresenter, IStackedViewPresenter
    {
        void IViewPresenter.PresentView(IPresentableView view)
        {
            Invoke(new object[] { view } );
        }

        void IStackedViewPresenter.PushView(IPresentableView view)
        {
            Invoke(new object[] { view });
        }

        void IStackedViewPresenter.DismissView(IPresentableView view)
        {
            Invoke(new object[] { view });
        }
    }
}
