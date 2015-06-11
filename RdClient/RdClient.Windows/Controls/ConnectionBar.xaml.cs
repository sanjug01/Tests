using RdClient.Shared.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    public sealed partial class ConnectionBar : UserControl
    {

        private ConnectionBarViewModel ViewModel
        {
            get { return DataContext as ConnectionBarViewModel; }
        }
        public ConnectionBar()
        {
            this.InitializeComponent();
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            ViewModel.MoveConnectionBar(e.Delta.Translation.X, this.ItemsControl.ActualWidth);            
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.05;
        }

    }
}
