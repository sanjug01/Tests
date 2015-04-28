namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionFailureViewModel
    {
        private readonly RdpDisconnectCode _failureCode;
        private readonly ICommand _dismiss;

        public RdpDisconnectCode FailureCode
        {
            get { return _failureCode; }
        }

        public ICommand Dismiss
        {
            get { return _dismiss; }
        }

        public RemoteSessionFailureViewModel(RdpDisconnectCode failureCode, Action dismissAction)
        {
            Contract.Requires(null != dismissAction);

            _failureCode = failureCode;
            _dismiss = new RelayCommand(param => dismissAction());
        }
    }
}
