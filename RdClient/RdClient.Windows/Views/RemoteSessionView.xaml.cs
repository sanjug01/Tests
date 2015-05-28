// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using Windows.UI.Xaml.Controls;

    public sealed partial class RemoteSessionView : UserControl, IPresentableView
    {
        public RemoteSessionView()
        {
            this.InitializeComponent();
        }
        //
        // IPresentableView interface
        //
        IViewModel IPresentableView.ViewModel
        {
            get { return (IViewModel)this.DataContext; }
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

        private void UserControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ((RemoteSessionViewModel)DataContext).ScrollBarModel.HorizontalScrollBarWidth = this.HorizontalScrolbar.ActualHeight;
            ((RemoteSessionViewModel)DataContext).ScrollBarModel.VerticalScrollBarWidth = this.VerticalScrolbar.ActualWidth;
        }
    }
}
