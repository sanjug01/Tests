using RdClient.Shared.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    public sealed partial class ConnectionBar : UserControl
    {

        private const double _decelration = 0.05;
        private Pointer _pointer;

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointer = e.Pointer;
        }

        private ConnectionBarViewModel ViewModel
        {
            get { return DataContext as ConnectionBarViewModel; }
        }
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            ((ButtonBase)e.OriginalSource).ReleasePointerCaptures();
            this.CapturePointer(_pointer);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            ViewModel.MoveConnectionBar(e.Delta.Translation.X, this.ItemsControl.ActualWidth);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = _decelration;
        }

        public ConnectionBar()
        {
            this.InitializeComponent();
            this.AddHandler(PointerPressedEvent, new PointerEventHandler(this.OnPointerPressed), true);
        }

    }
}
