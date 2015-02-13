namespace RdClient.Shared.Navigation.Extensions
{
    using System;

    public class DeferredExecutionException : Exception
    {
        public DeferredExecutionException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
