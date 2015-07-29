using System;

namespace RdClient.Shared.Navigation.Extensions
{
    // Wraps IDeferredExecution so that calls to Defer are synchronized with a thread lock
    public interface ISynchronizedDeferrer
    {
        bool TryDeferToUI(Action action);

        void DeferToUI(Action action);
    }
}
