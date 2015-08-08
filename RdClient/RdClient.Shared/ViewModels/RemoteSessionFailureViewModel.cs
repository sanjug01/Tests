namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionFailureViewModel : IBellyBandViewModel
    {
        private readonly ICommand _dismiss;

        public ICommand Dismiss
        {
            get { return _dismiss; }
        }

        public RemoteSessionFailureViewModel(Action dismissAction)
        {
            Contract.Requires(null != dismissAction);

            _dismiss = new RelayCommand(param => dismissAction());
        }

        void IBellyBandViewModel.Terminate()
        {
            this.Dismiss?.Execute(null);
        }
    }
}
