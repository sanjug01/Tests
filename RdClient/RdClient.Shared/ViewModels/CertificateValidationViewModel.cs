using RdClient.Shared.CxWrappers;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public sealed class CertificateValidationViewModelArgs
    {
        public CertificateValidationViewModelArgs(string host, IRdpCertificate certificate)
        {
            this.Host = host;
            this.Certificate = certificate;
        }

        public string Host { get; private set; }
        public IRdpCertificate Certificate { get; private set; }
    }

    public sealed class CertificateValidationResult
    {
        public enum CertificateTrustLevel
        {
            Denied,
            AcceptedOnce,
            AcceptedAlways
        }

        public CertificateValidationResult(CertificateTrustLevel result)
        {
            this.Result = result;
        }

        public CertificateTrustLevel Result { get; private set; }
    }

    public class CertificateValidationViewModel : ViewModelBase, ICertificateValidationViewModel
    {
        private string _host;
        private IRdpCertificate _certificate;
        private bool _isExpandedView;
        private bool _rememberChoice;

        private readonly RelayCommand _acceptCertificateCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _showDetailsCommand;
        private readonly RelayCommand _hideDetailsCommand;

        public CertificateValidationViewModel()
        {
            _acceptCertificateCommand = new RelayCommand((o) => AcceptCertificateCommandExecute());
            _cancelCommand = new RelayCommand((o) => CancelCommandExecute());
            _showDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = true; });
            _hideDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = false; });
        }

        public ICommand AcceptCertificate { get { return _acceptCertificateCommand; } }
        public ICommand ShowDetails { get { return _showDetailsCommand; } }
        public ICommand HideDetails { get { return _hideDetailsCommand; } }
        public ICommand Cancel { get { return _cancelCommand; } }

        public string Host
        {
            get { return _host; }
            private set { SetProperty(ref _host, value); }
        }

        public IRdpCertificate Certificate
        {
            get { return _certificate; }
            private set { SetProperty(ref _certificate, value); }            
        }

        public bool IsExpandedView
        {
            get { return _isExpandedView; }
            set { SetProperty(ref _isExpandedView, value); }
        }

        public bool RememberChoice
        {
            get { return _rememberChoice; }
            set { SetProperty(ref _rememberChoice, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is CertificateValidationViewModelArgs);
            CertificateValidationViewModelArgs args = activationParameter as CertificateValidationViewModelArgs;
            Contract.Assert(null != args.Certificate);
            
            this.Certificate = args.Certificate;
            this.Host = args.Host;
            this.RememberChoice = false;
            this.IsExpandedView = false;
        }

        protected override void OnNavigatingBack(Navigation.IBackCommandArgs backArgs)
        {
            this.Cancel.Execute(null);
            backArgs.Handled = true;
        }

        private void AcceptCertificateCommandExecute()
        {
            var trustLevel = CertificateValidationResult.CertificateTrustLevel.AcceptedOnce;
            if (this.RememberChoice)
            {
                trustLevel = CertificateValidationResult.CertificateTrustLevel.AcceptedAlways;
            }
            Dismiss(trustLevel);
        }

        private void CancelCommandExecute()
        {
            Dismiss(CertificateValidationResult.CertificateTrustLevel.Denied);
        }

        private void Dismiss(CertificateValidationResult.CertificateTrustLevel trustLevel)
        {
            DismissModal(new CertificateValidationResult(trustLevel));
        }
    }
}
