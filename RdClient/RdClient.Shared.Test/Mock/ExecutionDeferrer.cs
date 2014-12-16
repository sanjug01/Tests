using RdClient.Shared.Navigation.Extensions;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class ExecutionDeferrer : MockBase, IExecutionDeferrer
    {
        public bool TryDeferToUI(Action action)
        {
            return (bool)Invoke(new object[] { action });
        }

        public void DeferToUI(Action action)
        {
            throw new NotImplementedException();
        }
    }
}
