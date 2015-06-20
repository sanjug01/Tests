using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class WindowSize : IWindowSize
    {
        Size IWindowSize.Size
        {
            get
            {
                Rect bounds = Window.Current.CoreWindow.Bounds;
                return new Size(bounds.Width, bounds.Height);
            }
        }
    }
}