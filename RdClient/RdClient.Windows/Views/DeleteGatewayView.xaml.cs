using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class DeleteGatewayView : Page, IPresentableView
    {
        public DeleteGatewayView()
        {
            this.InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            (this.ViewModel as DeleteGatewayViewModel).PresentableView = this;
        }

        public void Dismissing()
        {
            
        }
    }
}
