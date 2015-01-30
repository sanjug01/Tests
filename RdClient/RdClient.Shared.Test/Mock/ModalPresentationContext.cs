using RdClient.Shared.Navigation;

namespace RdClient.Shared.Test.Mock
{
    class ModalPresentationContext : IModalPresentationContext
    {
        public object Result { get; private set; }
        void IModalPresentationContext.Dismiss(object result)
        {
            this.Result = result;
        }      
    }
}
