using RdClient.Shared.CxWrappers;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public sealed class DesktopIdentityValidationViewModelArgs
    {
        public DesktopIdentityValidationViewModelArgs(string host)
        {
            this.Host = host;
        }

        public string Host { get; private set; }
    }

    public sealed class DesktopIdentityValidationResult
    {
        public enum IdentityTrustLevel
        {
            Denied,
            AcceptedOnce,
            AcceptedAlways
        }

        public DesktopIdentityValidationResult(IdentityTrustLevel result)
        {
            this.Result = result;
        }

        public IdentityTrustLevel Result { get; private set; }
    }

    public class DesktopIdentityValidationViewModel : ViewModelBase
    {
        private string _host;

        private readonly RelayCommand _acceptIdentityCommand;
        private readonly RelayCommand _acceptOnceCommand;
        private readonly RelayCommand _cancelCommand;

        public DesktopIdentityValidationViewModel()
        {
            _acceptIdentityCommand = new RelayCommand((o) => { DismissModal(new DesktopIdentityValidationResult(DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedAlways)); });
            _acceptOnceCommand = new RelayCommand((o) => { DismissModal(new DesktopIdentityValidationResult(DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedOnce)); });
            _cancelCommand = new RelayCommand((o) => { DismissModal(new DesktopIdentityValidationResult(DesktopIdentityValidationResult.IdentityTrustLevel.Denied)); });
        }

        public ICommand AcceptIdentityCommand { get { return _acceptIdentityCommand; } }
        public ICommand AcceptOnceCommand { get { return _acceptOnceCommand; } }
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is DesktopIdentityValidationViewModelArgs);
            DesktopIdentityValidationViewModelArgs args = activationParameter as DesktopIdentityValidationViewModelArgs;
            
            this.Host = args.Host;
        }

        protected override void OnNavigatingBack(Navigation.IBackCommandArgs backArgs)
        {
            this.CancelCommand.Execute(null);
            backArgs.Handled = true;
        }
    }
}
