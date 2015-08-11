namespace RdClient.Shared.Telemetry.Events
{
    public sealed class UserAction
    {
        public enum Action
        {
            CancelConnectingSession,
            SetMouseMode,
            SetTouchMode,
            CancelCredentials,
            CancelGatewayCredentials
        }

        public enum Source
        {
            RightSideBar,
            ConnectingSessionState,
            ReconnectingSessionState
        }

        public Action action;
        public Source source;
        public double duration;
    }
}
