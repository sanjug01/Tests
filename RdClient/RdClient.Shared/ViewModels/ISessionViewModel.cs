using Windows.UI.Xaml;
namespace RdClient.Shared.ViewModels
{
    using Windows.Foundation;
    public interface IElephantEarsViewModel
    {
        Visibility ElephantEarsVisible { get; set; }
    }

    public interface IZoomPanViewModel
    {
        Point TranslatePosition(Point visiblePoint);
    }
}
