namespace RdClient.Shared.Telemetry.Events
{
    public sealed class NavigateToView
    {
        public readonly string view;

        public NavigateToView(string viewName)
        {
            this.view = viewName;
        }
    }
}
