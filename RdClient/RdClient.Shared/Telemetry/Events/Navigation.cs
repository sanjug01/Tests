namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported upon navigating to a new view or presenting a new modal/accessory view.
    /// </summary>
    public sealed class Navigation
    {
        public enum NavigationType
        {
            Present,
            Modal,
            Accessory
        }

        public Navigation(string viewName, NavigationType type)
        {
            this.view = viewName;
            this.type = type;
        }

        /// <summary>
        /// Name of the new view to that the application has navigated.
        /// </summary>
        public readonly string view;

        /// <summary>
        /// Type of the navigation event.
        /// </summary>
        public readonly NavigationType type;
    }
}
