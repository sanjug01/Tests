using Windows.UI.Xaml.Controls;

using RdClient.Navigation;

namespace RdClient.Views
{
    public sealed partial class ConnectionCenterView : Page, IPresentableView
    {
        public ConnectionCenterView()
        {
            this.InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter) { }

        public void Presenting(INavigationService navigationService, object activationParameter) { }

        public void Dismissing() { }
    }
}
