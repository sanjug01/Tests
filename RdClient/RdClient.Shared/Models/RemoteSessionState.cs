namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;

    public sealed class RemoteSessionState : MutableObject, IRemoteSessionState
    {
        private readonly IDeferredExecution _deferredExecution;
        private SessionState _state;
        private RdpDisconnectCode _disconnectCode;
        private int _reconnectAttempt;

        public RemoteSessionState(IDeferredExecution deferredExecution)
        {
            _deferredExecution = deferredExecution;
        }

        public void SetState(SessionState newState)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _state, newState, "State"));
        }

        public void SetDisconnectCode(RdpDisconnectCode code)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _disconnectCode, code, "DisconnectCode"));
        }

        public void SetReconnectAttempt(int reconnectAttempt)
        {
            _deferredExecution.Defer(() => this.SetProperty(ref _reconnectAttempt, reconnectAttempt, "ReconnectAttempt"));
        }

        SessionState IRemoteSessionState.State
        {
            get { return _state; }
        }

        RdpDisconnectCode IRemoteSessionState.DisconnectCode
        {
            get { return _disconnectCode; }
        }

        int IRemoteSessionState.ReconnectAttempt
        {
            get { return _reconnectAttempt; }
        }
    }
}
