namespace RdClient.Shared.Helpers
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Interface of an object that can defer execution of action delegates until some
    /// conditions specific to the nature of the object are met.
    /// </summary>
    [ContractClass(typeof(Contracts.IDeferredExecutionContract))]
    public interface IDeferredExecution
    {
        /// <summary>
        /// Defer execution of an action delegate.
        /// </summary>
        /// <param name="action">Delegate that will be executed at some point.</param>
        /// <remarks>The action delegate may be executed before retuning from Defer on the same thread,
        /// or on some other thread. Deferring of execution is a fire-and-forget action.</remarks>
        void Defer(Action action);
    }
}
