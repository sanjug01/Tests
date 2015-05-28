using System;

namespace RdClient.Shared.Navigation.Extensions
{
    public interface IExecutionDeferrer
    {
        bool TryDeferToUI(Action action);

        void DeferToUI(Action action);
    }
}
