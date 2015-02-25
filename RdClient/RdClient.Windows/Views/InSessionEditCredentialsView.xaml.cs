// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class InSessionEditCredentialsView : UserControl, IPresentableView
    {
        public InSessionEditCredentialsView()
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

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
        }

        void IPresentableView.Dismissing()
        {
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //
            // Navigation service disables modal views before cross-fade animating them.
            // Because of that, all controls are disabled when the new view is shown, and focus cannot be set
            // to a disabled control. Waiting until the view becomes enabled and setting focus then helps.
            //
            if ((bool)e.NewValue)
            {
                if (string.IsNullOrWhiteSpace(this.UserName.Text))
                    this.UserName.Focus(FocusState.Programmatic);
                else
                    this.Password.Focus(FocusState.Programmatic);
            }
        }
    }
}
