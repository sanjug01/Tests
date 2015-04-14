using RdClient.Shared.Navigation;

namespace RdClient.Shared.Test.Mock
{
    class ModalPresentationContext : IStackedPresentationContext
    {
        public object Result { get; private set; }
        void IStackedPresentationContext.Dismiss(object result)
        {
            this.Result = result;
        }      
    }
}
