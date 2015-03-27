namespace RdClient.Shared.Helpers
{
    using System;
    using System.Threading.Tasks;

    public class TaskExecutor : IDeferredExecution
    {
        public void Defer(Action action)
        {
            Task.Run(action);
        }
    }
}
