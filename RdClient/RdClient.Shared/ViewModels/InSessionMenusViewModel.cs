using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// View model of the modal dialog shown over the remote session view when the "ellipsis"
    /// button's been clicked in the connection bar.
    /// </summary>
    public sealed class InSessionMenusViewModel : ViewModelBase
    {
        private readonly RelayCommand _cancel;
        private readonly RelayCommand _disconnect;

        public InSessionMenusViewModel()
        {
            _cancel = new RelayCommand(this.OnCancel);
            _disconnect = new RelayCommand(this.OnDisconnect, this.CanDisconnect);
        }

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public ICommand Disconnect
        {
            get { return _disconnect; }
        }

        protected override void OnPresenting(object activationParameter)
        {
            base.OnPresenting(activationParameter);
        }

        protected override void OnDismissed()
        {
            base.OnDismissed();
        }

        private void OnCancel(object parameter)
        {
            this.DismissModal(null);
        }

        private void OnDisconnect(object parameter)
        {
        }

        private bool CanDisconnect(object parameter)
        {
            return true;
        }
    }
}
