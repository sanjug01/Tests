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

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        public CertificateValidationViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute,
                o => this.SaveCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            this.IsExpandedView = false;
        }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string Host
        {
            get { return _host; }
            set
            {
                SetProperty(ref _host, value, "Host");
                _saveCommand.EmitCanExecuteChanged();
            }
        }

        public IRdpCertificate Certificate
        {
            get { return _certificate; }
            private set
            {
                SetProperty(ref _certificate, value, "Certificate");
            }
        }

        public string CertificateIssuer { get { return "TestIssuer: MyPC"; } }
        public string CertificateThumbprint { get { return "TestThumbprint: 1A:1B:1C:1A:1B:1C:1A:1B:1C:1A:1B:1C:1A:1B:1C:1A:1B:1C:1A:1B:1C:1A:1B:1C"; } }
        public string CertificateValidBefore { get { return "TestValidBefore: 10/01/2019 00:23"; } }
        public string CertificateValidAfter { get { return "TestValidAfter: 01/01/1900 12:45"; } }
        public string CertificateError { get { return "TestError"; } }


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

        private void SaveCommandExecute(object o)
        {
            DismissModal(true);
        }

        private bool SaveCommandCanExecute()
        {
            return true;
        }


        private void CancelCommandExecute(object o)
        {
            DismissModal(false);
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as CertificateValidationViewModelArgs);
            CertificateValidationViewModelArgs args = activationParameter as CertificateValidationViewModelArgs;

            this._certificate = args.Certificate;
            this.Host = args.Host;
            this.IsExpandedView = false;
            this.IsDontAsk = false;
        }
    }
}
