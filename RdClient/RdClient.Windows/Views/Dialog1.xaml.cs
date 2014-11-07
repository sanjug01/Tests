using RdClient.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RdClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dialog1 : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public IViewModel ViewModel { get { return null; } }

        public Dialog1()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            _navigationService = navigationService;
        }

        public void Dismissing()
        {
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.DismissModalView(this);
        }
    }
}
