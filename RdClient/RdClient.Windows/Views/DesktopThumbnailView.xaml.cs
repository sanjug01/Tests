using Windows.Foundation;
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

        private void ThumbnailButton_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var element = sender as FrameworkElement;

            // update the position of the flyout
            Point pos = e.GetPosition(element);
            FlyoutTranslateTransform.X = pos.X;
            FlyoutTranslateTransform.Y = pos.Y;
            Flyout.ShowAttachedFlyout(this.FlyoutButton);
            
            e.Handled = true;
        }
    }
}
