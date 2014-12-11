using RdClient.Shared.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.ValidationRules;

using System.Collections.ObjectModel;
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

    public class CertificateValidationViewModel : ViewModelBase
    {
        private string _host;
        private IRdpCertificate _certificate;

        private bool _isExpandedView;

        private readonly RelayCommand _acceptCertificateCommand;
        private readonly RelayCommand _acceptOnceCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _showDetailsCommand;
        private readonly RelayCommand _hideDetailsCommand;

        public CertificateValidationViewModel()
        {
            _acceptCertificateCommand = new RelayCommand((o) => { DismissModal(new CertificateValidationResult(CertificateValidationResult.CertificateTrustLevel.AcceptedAlways)); });
            _acceptOnceCommand = new RelayCommand((o) => { DismissModal(new CertificateValidationResult(CertificateValidationResult.CertificateTrustLevel.AcceptedOnce)); });
            _cancelCommand = new RelayCommand((o) => { DismissModal(new CertificateValidationResult(CertificateValidationResult.CertificateTrustLevel.Denied)); });
            _showDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = true; });
            _hideDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = false; });

            this.IsExpandedView = false;

            // _errorStrings = new CertificateErrorString(new LocalizedString());

            this.CertificateErrorsList = new ObservableCollection<string>();
        }

        public ObservableCollection<string> CertificateErrorsList { get; set; }

        public ICommand AcceptCertificateCommand { get { return _acceptCertificateCommand; } }
        public ICommand AcceptOnceCommand { get { return _acceptOnceCommand; } }
        public ICommand ShowDetailsCommand { get { return _showDetailsCommand; } }
        public ICommand HideDetailsCommand { get { return _hideDetailsCommand; } }
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        public IRdpCertificate Certificate
        {
            get { return _certificate; }
            private set 
            { 
                SetProperty(ref _certificate, value);
                _acceptCertificateCommand.EmitCanExecuteChanged();
            }
        }

        public bool IsExpandedView
        {
            get { return _isExpandedView; }
            set { SetProperty(ref _isExpandedView, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is CertificateValidationViewModelArgs);
            CertificateValidationViewModelArgs args = activationParameter as CertificateValidationViewModelArgs;
            Contract.Assert(null != args.Certificate);
            
            this.Certificate = args.Certificate;
            this.Host = args.Host;
            this.IsExpandedView = false;
            LoadErrorStrings();
        }

        private void LoadErrorStrings()
        {
            Contract.Assert(null != this.Certificate);

            // Should use CertificateErrorString to load localizable strings
            CertificateErrorsList.Clear();

            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.Expired))
                this.CertificateErrorsList.Add("Certificate expired");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.NameMismatch))
                this.CertificateErrorsList.Add("Certificate name mismatch");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.UntrustedRoot))
                this.CertificateErrorsList.Add("Certificate untrusted");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.Revoked))
                this.CertificateErrorsList.Add("Certificate revoked");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.RevocationUnknown))
                this.CertificateErrorsList.Add("Certificate revocation unknown");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.CertOrChainInvalid))
                this.CertificateErrorsList.Add("Certificate or chain invalid");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.MismatchedCert))
                this.CertificateErrorsList.Add("Certificate mismatch");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.WrongEKU))
                this.CertificateErrorsList.Add("Certificate wrong EKU");
            if (CertificateErrorHelper.ErrorContainsFlag(this.Certificate.Error.ErrorFlags, CertificateErrors.Critical))
                this.CertificateErrorsList.Add("Certificate critical error");

            EmitPropertyChanged("CertificateErrorsList");
        }
    }
}
