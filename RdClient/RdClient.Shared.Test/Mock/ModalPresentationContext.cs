using RdClient.Shared.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    class ModalPresentationContext : MockBase, IStackedPresentationContext
    {
        void IStackedPresentationContext.Dismiss(object result)
        {
            Invoke(new object[] { result });
        }      
    }
}
