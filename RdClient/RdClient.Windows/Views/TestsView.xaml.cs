using RdClient.CxWrappers.Utils;
using RdClient.Shared.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class TestsView : Page, IPresentableView
    {
        public IViewModel ViewModel { get { return this.TestViewModel; } }

        public TestsView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RdpConnectionFactory factory = new RdpConnectionFactory();
            factory.SwapChainPanel = this.TestSwapChainPanel;

            this.TestViewModel.RdpConnectionFactory = factory;
        }

        private void DesktopsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // desktopsView.SelectedItems is not bindable - so we need to update the model on SelectionChanged
            this.TestViewModel.SelectedDesktops = this.desktopsView.SelectedItems;
        }

        private void DesktopsView_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FrameworkElement listView = sender as FrameworkElement;
            Flyout.ShowAttachedFlyout(listView);
        }
    }
}
