using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RdClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddUserView : Page, IPresentableView
    {
        public IViewModel ViewModel { get { return this.DataContext as IViewModel; } }
        public AddUserView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            (this.ViewModel as AddUserViewModel).PresentableView = this;
        }

        public void Dismissing()
        {
        }
    }
}
