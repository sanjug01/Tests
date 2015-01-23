using RdClient.Shared.Navigation;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : UserControl, IPresentableView
    {
        public IViewModel ViewModel { get { return this.DataContext as IViewModel; } }

        public SessionView()
        {
            this.InitializeComponent();
            SessionPanControl.HidePanControl();
        }

        void IPresentableView.Activating(object activationParameter)
        {            
        }

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
        }

        void IPresentableView.Dismissing()
        {
        }

        private void ZoomInButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SessionPanel.ZoomIn();
            SessionPanControl.ShowPanControl();
        }

        private void ZoomOutButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SessionPanel.ZoomOut();
            SessionPanControl.HidePanControl();
        }
    }
}
