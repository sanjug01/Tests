using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input;

namespace RdClient.Views
{
    public sealed partial class DesktopThumbnailView : UserControl
    {
        public DesktopThumbnailView()
        {
            this.InitializeComponent();
        }

        private void DesktopThumbnail_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(this.ThumbnailButton);
                if(ptrPt.Properties.IsRightButtonPressed)
                {
                    Flyout.ShowAttachedFlyout((FrameworkElement)sender);
                }
            }
        }
    }
}
