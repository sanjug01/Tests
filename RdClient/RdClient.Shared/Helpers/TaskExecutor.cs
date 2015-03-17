namespace RdClient.Shared.Helpers
{
    using System;
    using System.Threading.Tasks;

    class TaskExecutor : IDeferredExecution
    {
        public void Defer(Action action)
        {
            Task.Run(action);
        }
    }
}
