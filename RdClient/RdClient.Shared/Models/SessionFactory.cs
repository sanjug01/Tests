namespace RdClient.Shared.Models
{
    public sealed class SessionFactory : ISessionFactory
    {
        private Helpers.IDeferredExecution _deferredExecution;

        Helpers.IDeferredExecution ISessionFactory.DeferedExecution
        {
            get { return _deferredExecution; }
            set { _deferredExecution = value; }
        }
    }
}
