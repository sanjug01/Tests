using RdClient.Navigation;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View1 : Page, IPresentableView
    {
        public View1()
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
    }
}
