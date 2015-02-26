namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AddOrEditWorkspaceView : Page, IPresentableView
    {
        public AddOrEditWorkspaceView()
        {
            this.InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter)
        { }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            (this.ViewModel as AddOrEditWorkspaceViewModel).PresentableView = this;
        }

        public void Dismissing()
        { }
    }
}
