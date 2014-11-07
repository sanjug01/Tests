using RdClient.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DialogMessage : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public IViewModel ViewModel { get { return null; } }

        public DialogMessage()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            DialogMessageViewModel dmvm = Resources["DialogMessageViewModel"] as DialogMessageViewModel;
            dmvm.DialogView = this;
            dmvm.Presenting(navigationService, activationParameter);
        }

        public void Dismissing()
        {
        
        }
    }
}
