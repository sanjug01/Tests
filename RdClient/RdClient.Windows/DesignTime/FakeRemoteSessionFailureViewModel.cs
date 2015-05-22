namespace RdClient.DesignTime
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.ViewModels;
    using System.Windows.Input;

    public sealed class FakeRemoteSessionFailureViewModel
    {
        public RdpDisconnectCode FailureCode
        {
            get { return RdpDisconnectCode.GenericNetworkError; }
        }

        public ICommand Dismiss
        {
            get { return new RelayCommand(o => { }, o => true); }
        }        
    }
}
