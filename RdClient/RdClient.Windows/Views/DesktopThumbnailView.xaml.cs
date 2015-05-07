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
            Flyout.ShowAttachedFlyout(element);
            e.Handled = true;
        }
    }
}
