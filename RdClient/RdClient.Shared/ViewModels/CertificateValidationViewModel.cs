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

    public class CertificateValidationViewModel : ViewModelBase
    {
        private string _host;
        private IRdpCertificate _certificate;

        private bool _isExpandedView;

        private readonly RelayCommand _acceptCertificateCommand;
        private readonly RelayCommand _acceptOnceCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _showDetailsCommand;

        public CertificateValidationViewModel()
        {
            _acceptCertificateCommand = new RelayCommand(AcceptCertificateExecute);
            _acceptOnceCommand = new RelayCommand(AcceptCertificateOnceExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _showDetailsCommand = new RelayCommand(ShowDetailsCommandExecute);

            this.IsExpandedView = false;
        }

        public ICommand AcceptCertificateCommand { get { return _acceptCertificateCommand; } }
        public ICommand AcceptOnceCommand { get { return _acceptOnceCommand; } }
        public ICommand ShowDetailsCommand { get { return _showDetailsCommand; } }


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

        /// <summary>
        /// Show or hide the details of the certificate
        /// </summary>
        /// <param name="o">true or false</param>
        private void ShowDetailsCommandExecute(object o)
        {
            Contract.Assert(null != o);
            bool showFlag = false;
            bool.TryParse(o.ToString(), out showFlag);

            this.IsExpandedView = showFlag;

        }

        /// <summary>
        /// Accept certificate, and save it for future sessions
        ///     It will dismiss the dialog
        /// </summary>
        /// <param name="o">param object</param>
        private void AcceptCertificateExecute(object o)
        {
            // should save certificate 
            DismissModal(true);
        }

        /// <summary>
        /// Accept certificate, but do not save it for future sessions
        ///     It will dismiss the dialog
        /// </summary>
        /// <param name="o">param object</param>
        private void AcceptCertificateOnceExecute(object o)
        {
            DismissModal(true);
        }
        
        private void CancelCommandExecute(object o)
        {
            DismissModal(false);
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is CertificateValidationViewModelArgs);
            CertificateValidationViewModelArgs args = activationParameter as CertificateValidationViewModelArgs;
            Contract.Assert(null != args.Certificate);
            
            this.Certificate = args.Certificate;
            this.Host = args.Host;
            this.IsExpandedView = false;
        }
    }
}
