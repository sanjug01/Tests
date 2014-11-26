namespace RdClient.Shared.Navigation.Contracts
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    [DebuggerNonUserCode] // exclude from code coverage
    [ContractClassFor(typeof(IPresentationCompletion))]
    abstract class IPresentationCompletionContract : IPresentationCompletion
    {
        public void Completed(IPresentableView view, object result)
        {
            Contract.Requires(null != view);
            throw new NotImplementedException();
        }
    }
}
