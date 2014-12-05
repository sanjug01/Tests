using RdClient.Shared.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using RdClient.Shared.ValidationRules;

using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    // TODO 
    public class CertificateValidationViewModelArgs
    {
        public CertificateValidationViewModelArgs(string host, IRdpCertificate certificate)
        {
            this.Host = host;
            this.Certificate = certificate;
        }

        public string Host { get; private set; }
        public IRdpCertificate Certificate { get; private set; }
    }
    public class CertificateValidationViewModel : ViewModelBase
    {
        private string _host;
        private IRdpCertificate _certificate;
        private bool _isDontAsk;

        private bool _isExpandedView;

        private readonly RelayCommand _acceptCertificateCommand;
        private readonly RelayCommand _cancelCommand;

        public CertificateValidationViewModel()
        {
            _acceptCertificateCommand = new RelayCommand(AcceptCertificateExecute,
                o => this.AcceptCertificateCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            this.IsExpandedView = false;
        }

        public ICommand AcceptCertificateCommand { get { return _acceptCertificateCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value, "Host"); }
        }

        public IRdpCertificate Certificate
        {
            get { return _certificate; }
            private set { SetProperty(ref _certificate, value, "Certificate"); }
        }

        public string CertificateIssuer { get; private set; }
        public string CertificateThumbprint { get; private set; }
        public string CertificateValidBefore { get; private set; }
        public string CertificateValidAfter { get; private set; }
        public string CertificateSubject { get; private set; }
        public string CertificateError { get; private set; }

        public bool IsExpandedView
        {
            get { return _isExpandedView; }
            set { SetProperty(ref _isExpandedView, value, "IsExpandedView"); }
        }

        public bool IsDontAsk
        {
            get { return _isDontAsk; }
            set { SetProperty(ref _isDontAsk, value, "IsDontAsk"); }
        }

        private void AcceptCertificateExecute(object o)
        {
            DismissModal(true);
        }

        private bool AcceptCertificateCommandCanExecute()
        {
            return (null != this.Certificate);
        }


        private void CancelCommandExecute(object o)
        {
            DismissModal(false);
        }

        private void ResetCertificateData()
        {
            if(null != this.Certificate)
            {
                this.CertificateIssuer = Certificate.Issuer;
                this.CertificateValidAfter = Certificate.ValidFrom.LocalDateTime.ToString() ;
                this.CertificateValidBefore = Certificate.ValidTo.LocalDateTime.ToString() ;
                this.CertificateThumbprint = Certificate.SerialNumber.ToString();
                this.CertificateSubject = Certificate.Subject;
            }
            else
            {
                // TODO: should not allow null certificates
                this.CertificateIssuer = "<None>";
                this.CertificateValidAfter  = "N/A";
                this.CertificateValidBefore  = "N/A";
                this.CertificateThumbprint = "N/A";
                this.CertificateSubject = "<No subject>";
            }

            this.IsExpandedView = false;
            this.IsDontAsk = false;
            _acceptCertificateCommand.EmitCanExecuteChanged();
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as CertificateValidationViewModelArgs);
            CertificateValidationViewModelArgs args = activationParameter as CertificateValidationViewModelArgs;
            
            this.Certificate = args.Certificate;
            this.Host = args.Host;
            this.ResetCertificateData();
        }
    }
}
