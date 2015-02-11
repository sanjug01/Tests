namespace RdClient.Shared.Helpers
{
    using System;
    using System.Diagnostics.Contracts;

    public class ContractAssertionException : Exception
    {
        private readonly string _condition;

        public string Condition { get { return _condition; } }

        public ContractAssertionException(ContractFailedEventArgs e) : base(e.Message, e.OriginalException)
        {
            _condition = e.Condition;
        }
    }
}
