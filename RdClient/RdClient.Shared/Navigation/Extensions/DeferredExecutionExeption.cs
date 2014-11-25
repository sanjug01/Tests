namespace RdClient.Shared.Navigation.Extensions
{
    using System;

    public class DeferredExecutionExeption : Exception
    {
        public DeferredExecutionExeption(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
