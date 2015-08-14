using RdClient.Shared.Navigation.Extensions;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class ExecutionDeferrer : MockBase, IExecutionDeferrer
    {
        public bool TryDeferToUI(Action action)
        {
            action();
            return (bool)Invoke(new object[] { action });
        }

        public void DeferToUI(Action action)
        {
            action();
            Invoke(new object[] { action });
        }
    }
}
