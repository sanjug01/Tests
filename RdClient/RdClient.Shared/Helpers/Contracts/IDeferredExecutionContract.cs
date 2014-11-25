namespace RdClient.Shared.Helpers.Contracts
{
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDeferredExecution))]
    abstract class IDeferredExecutionContract : IDeferredExecution
    {
        public void Defer(System.Action action)
        {
            Contract.Requires(null != action);
            throw new System.NotImplementedException();
        }
    }
}
