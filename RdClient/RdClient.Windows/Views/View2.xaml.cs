using RdClient.Navigation;
using System;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View2 : Page, IPresentableView
    {
        public View2()
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
