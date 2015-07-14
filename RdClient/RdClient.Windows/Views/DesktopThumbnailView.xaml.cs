using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RdClient.Views
{
    public sealed partial class DesktopThumbnailView : UserControl
    {
        // size offsets for the margin, 
        // when converting from itemWidth and itemHeight to grid size
        private const double _widthOffset = 4;
        private const double _heightOffset = 4;

        public DesktopThumbnailView()
        {
            this.InitializeComponent();
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TileGrid.Width = e.NewSize.Width - _widthOffset;
            this.TileGrid.Height = e.NewSize.Height - _heightOffset;
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
