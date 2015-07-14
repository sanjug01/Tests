using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RdClient.Views
{
    public sealed partial class DesktopThumbnailView : UserControl
    {

        public DesktopThumbnailView()
        {
            this.InitializeComponent();
            this.SizeChanged += DesktopThumbnailView_SizeChanged;
        }

        private void DesktopThumbnailView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TileGrid.Width = e.NewSize.Width - 4;
            this.TileGrid.Height = e.NewSize.Height - 4;
        }

        private void ThumbnailButton_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var element = sender as FrameworkElement;

            // update the position of the flyout
            Point pos = e.GetPosition(element);
            FlyoutTranslateTransform.X = pos.X;
            FlyoutTranslateTransform.Y = pos.Y;
            FlyoutBase.ShowAttachedFlyout(FlyoutButton);
            
            e.Handled = true;
        }

    }
}
