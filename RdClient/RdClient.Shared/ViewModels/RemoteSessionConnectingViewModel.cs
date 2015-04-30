namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionConnectingViewModel
    {
        private readonly RelayCommand _cancel;
        private Action _cancelAction;

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public RemoteSessionConnectingViewModel(Action cancelAction)
        {
            Contract.Assert(null != cancelAction);

            _cancelAction = cancelAction;
            _cancel = new RelayCommand(this.CancelConnection, this.CanCancelConnection);
        }

        private void CancelConnection(object parameter)
        {
            Contract.Assert(null != _cancelAction);
            _cancelAction();
            _cancelAction = null;
            _cancel.EmitCanExecuteChanged();
        }

        private bool CanCancelConnection(object parameter)
        {
            return null != _cancelAction;
        }
    }
}
