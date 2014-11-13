using RdClient.CxWrappers.Utils;
using RdClient.Navigation;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddOrEditDesktopView : Page, IPresentableView
    {
        public IViewModel ViewModel { get { return (IViewModel)this.DataContext; } }

        public AddOrEditDesktopView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            (this.DataContext as AddOrEditDesktopViewModel).PresentableView = this;
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
