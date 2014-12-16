using RdClient.CxWrappers.Utils;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : UserControl, IPresentableView
    {
        public IViewModel ViewModel { get { return this.DataContext as IViewModel; } }

        public SessionView()
        {
            this.InitializeComponent();
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
    }
}
