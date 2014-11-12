namespace RdClient.Shared.ViewModels
{
    public enum ViewOrientation
    {
        Portrait,
        Landscape
    }

    public interface ILayoutAwareViewModel
    {
        void OrientationChanged(ViewOrientation orientation);
    }
}
