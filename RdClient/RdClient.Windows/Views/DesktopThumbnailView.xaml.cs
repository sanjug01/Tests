using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class DesktopThumbnailView : UserControl
    {
        public DesktopThumbnailView()
        {
            this.InitializeComponent();
        }

        private void Button_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout((FrameworkElement) sender);
        }
    }
}
