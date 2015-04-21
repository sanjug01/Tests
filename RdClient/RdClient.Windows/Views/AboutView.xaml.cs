// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AboutView : UserControl, IPresentableView
    {
        public AboutView()
        {
            this.InitializeComponent();
        }

        IViewModel IPresentableView.ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        void IPresentableView.Activating(object activationParameter)
        {
        }

        void IPresentableView.Dismissing()
        {
        }

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
        }
    }
}
