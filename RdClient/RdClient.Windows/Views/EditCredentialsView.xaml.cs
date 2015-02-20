// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class EditCredentialsView : UserControl, IPresentableView
    {
        public EditCredentialsView()
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

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
            Dispatcher.AcceleratorKeyActivated += this.OnAcceleratorKeyActivated;
        }

        void IPresentableView.Dismissing()
        {
            Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
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

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            switch(e.VirtualKey)
            {
                case VirtualKey.Escape:
                    this.Cancel.Invoke(e);
                    break;

                case VirtualKey.Enter:
                    this.Submit.Invoke(e);
                    break;
            }
        }
    }
}
