namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionInterruptionViewModel : MutableObject
    {
        private readonly RelayCommand _cancel;
        private readonly IRemoteSession _session;
        private InterruptedSessionContinuation _continuation;

        private int _reconnectAttempt;

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public int ReconnectAttempt
        {
            get { return _reconnectAttempt; }
            private set { SetProperty(ref _reconnectAttempt, value); }
        }

        public string HostName { get; private set; }

        public RemoteSessionInterruptionViewModel(IRemoteSession session, InterruptedSessionContinuation continuation)
        {
            _session = session;
            _continuation = continuation;
            _cancel = new RelayCommand(this.CancelReconnect, this.CanCancelReconnect);
            _session.State.PropertyChanged += this.OnSessionStatePropertyChanged;
            this.ReconnectAttempt = session.State.ReconnectAttempt;
            this.HostName = _session.HostName;

        }

        private void CancelReconnect(object parameter)
        {
            Contract.Assert(null != _continuation);
            //
            // Execute the action and clear it, reconnect can only be cancelled once.
            // Clearing _cancelAction also disables the button bound to the command.
            //
            _continuation.Cancel();
            _continuation = null;
            _cancel.EmitCanExecuteChanged();
            _session.State.PropertyChanged -= this.OnSessionStatePropertyChanged;
        }

        private bool CanCancelReconnect(object parameter)
        {
            return null != _continuation;
        }

        private void OnSessionStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.EndsWith("ReconnectAttempt"))
            {
                Contract.Assert(sender is IRemoteSessionState);

                IRemoteSessionState state = (IRemoteSessionState)sender;
                this.ReconnectAttempt = state.ReconnectAttempt;
            }
        }
    }
}
