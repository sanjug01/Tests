namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;

    public sealed class RemoteSessionState : MutableObject, IRemoteSessionState
    {
        private readonly IDeferredExecution _deferredExecution;
        private SessionState _state;

        public RemoteSessionState(IDeferredExecution deferredExecution)
        {
            _deferredExecution = deferredExecution;
        }

        public void SetState(SessionState newState)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _state, newState));
        }

        SessionState IRemoteSessionState.State
        {
            get { return _state; }
        }
    }
}
