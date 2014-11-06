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
        private INavigationService _navigationService;
        private object _activationParameter;

        public AddOrEditDesktopView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);

            _navigationService = navigationService;
            _activationParameter = activationParameter;
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Contract.Assert(_activationParameter != null);
            IAddOrEditDesktopViewModel vm = Resources["AddOrEditDesktopViewModel"] as IAddOrEditDesktopViewModel;

            if (null == _activationParameter )
            {
                // add
                vm.IsAddingDesktop = true;
            }
            else
            {
                //edit
                vm.IsAddingDesktop = false;
            }

            // Desktop desktop = _activationParameter as Desktop;

            vm.NavigationService = _navigationService;            
        }

        private void UsersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void Gateways_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }
    }
}
