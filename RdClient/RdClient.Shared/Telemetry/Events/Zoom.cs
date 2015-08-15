namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported when user zooms in or out in the session UI.
    /// </summary>
    public sealed class Zoom
    {
        public enum ZoomAction
        {
            ZoomIn,
            ZoomOut
        }

        public enum Source
        {
            ConnectionBar
        }

        public Zoom(ZoomAction action, Source source)
        {
            this.action = action;
            this.source = source;
        }

        /// <summary>
        /// Zoom action.
        /// </summary>
        public readonly ZoomAction action;

        /// <summary>
        /// Source of the action in the application UI.
        /// </summary>
        public readonly Source source;
    }
}
