namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using Windows.UI.Xaml.Controls;

    public sealed partial class InSessionMenusView : UserControl, IPresentableView
    {
        public InSessionMenusView()
        {
            this.InitializeComponent();
        }

        IViewModel IPresentableView.ViewModel
        {
            get { return (IViewModel)this.DataContext; }
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
