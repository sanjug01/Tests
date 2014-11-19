using RdClient.Shared.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class ViewPresenter : MockBase, IViewPresenter
    {
        public void PresentView(IPresentableView view)
        {
            Invoke(new object[] { view } );
        }

        public void PushModalView(IPresentableView view)
        {
            Invoke(new object[] { view });
        }

        public void DismissModalView(IPresentableView view)
        {
            Invoke(new object[] { view });
        }
    }
}
