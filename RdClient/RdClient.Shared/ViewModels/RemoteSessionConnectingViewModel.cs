namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionConnectingViewModel : IBellyBandViewModel
    {
        private readonly RelayCommand _cancel;
        private Action _cancelAction;

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public string HostName { get; private set; }

        public RemoteSessionConnectingViewModel(string hostName, Action cancelAction)
        {
            Contract.Assert(null != cancelAction);

            this.HostName = hostName;
            _cancelAction = cancelAction;
            _cancel = new RelayCommand(this.CancelConnection, this.CanCancelConnection);
        }

        private void CancelConnection(object parameter)
        {
            Contract.Assert(null != _cancelAction);
            _cancelAction();
            //
            // Clear the cancellation action and emit the "can execute changed" event from the command
            // to let the UI update the button. Cancellation command may be executed only once.
            //
            _cancelAction = null;
            _cancel.EmitCanExecuteChanged();
        }

        private bool CanCancelConnection(object parameter)
        {
            return null != _cancelAction;
        }

        void IBellyBandViewModel.Terminate()
        {
            if(this.CanCancelConnection(null))
            {
                this.CancelConnection(null);
            }
        }
    }
}
