namespace RdClient.Shared.Helpers.Contracts
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDeferredExecution))]
    abstract class IDeferredExecutionContract : IDeferredExecution
    {
        [DebuggerNonUserCode]
        public void Defer(System.Action action)
        {
            Contract.Requires(null != action);
            throw new System.NotImplementedException();
        }
    }
}
