namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;

    public sealed class RemoteSessionState : MutableObject, IRemoteSessionState
    {
        private readonly IDeferredExecution _deferredExecution;
        private SessionState _state;
        private RdpDisconnectCode _disconnectCode;

        public RemoteSessionState(IDeferredExecution deferredExecution)
        {
            _deferredExecution = deferredExecution;
        }

        public void SetState(SessionState newState)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _state, newState));
        }

        public void SetDisconnectCode(RdpDisconnectCode code)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _disconnectCode, code));
        }

        SessionState IRemoteSessionState.State
        {
            get { return _state; }
        }

        RdpDisconnectCode IRemoteSessionState.DisconnectCode
        {
            get { return _disconnectCode; }
        }
    }
}
