namespace RdClient.Shared.Models
{
    public sealed class InterruptedSessionContinuation
    {
        private readonly IRemoteSession _session;

        public InterruptedSessionContinuation(IRemoteSession session)
        {
            _session = session;
        }

        public void Cancel()
        {
            _session.Disconnect();
        }
    }
}
