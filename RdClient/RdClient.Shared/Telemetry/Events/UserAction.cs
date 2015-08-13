namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported when user performs an action in the UI.
    /// </summary>
    public sealed class UserAction
    {
        public enum ActionType
        {
            /// <summary>
            /// User has cancelled a connecting session.
            /// </summary>
            CancelConnectingSession,

            /// <summary>
            /// User has changed session input mode to "Mouse" (A.K.A. "Pointer").
            /// </summary>
            SetMouseMode,

            /// <summary>
            /// User has changed session input mode to "Touch"
            /// </summary>
            SetTouchMode,

            /// <summary>
            /// User has cancelled the credentials prompt.
            /// </summary>
            CancelCredentials,

            /// <summary>
            /// User has cancelled the gateway credentials prompt.
            /// </summary>
            CancelGatewayCredentials
        }

        public enum Source
        {
            RightSideBar,
            ConnectingSessionState,
            ReconnectingSessionState
        }

        public UserAction(ActionType action, Source source, double timeMark)
        {
            this.action = action;
            this.source = source;
            this.timeMark = timeMark;
        }

        /// <summary>
        /// Type (name) of the action from the ActionType enum.
        /// </summary>
        public readonly ActionType action;

        /// <summary>
        /// Source of the action - part of the UI in that the command has originated.
        /// </summary>
        public readonly Source source;

        /// <summary>
        /// Time mark in seconds relative to beginning of some process meaningful for the action type.
        /// </summary>
        public readonly double timeMark;
    }
}
