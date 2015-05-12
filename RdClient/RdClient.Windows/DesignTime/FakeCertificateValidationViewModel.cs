using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.CxWrappers;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeCertificateError : IRdpCertificateError
    {
        private int _code;
        private CertificateError _flags;
        private ServerCertificateErrorSource _source;

        public FakeCertificateError(int code, CertificateError flags, ServerCertificateErrorSource source)
        {
            _code = code;
            _flags = flags;
            _source = source;
        }
        public int ErrorCode { get { return _code; } }

        public CertificateError ErrorFlags { get { return _flags; } }

        public ServerCertificateErrorSource ErrorSource { get { return _source; } }
    }

    public class FakeRdpCertificate : IRdpCertificate
    {
        public IRdpCertificateError Error
        {
            get
            {
                CertificateError errors =
                    CertificateError.CertOrChainInvalid |
                    CertificateError.Critical |
                    CertificateError.Expired;
                return new FakeCertificateError(911, errors, ServerCertificateErrorSource.CredSSP);
            }
        }

        public string FriendlyName
        {
            get { return "This is a certificate"; }
        }

        public bool HasPrivateKey
        {
            get { return false; }
        }

        public bool IsStronglyProtected
        {
            get { return false; }
        }

        public string Issuer
        {
            get { return "Corporation of certificate issuing incorporated"; }
        }

        public byte[] SerialNumber
        {
            get { return new byte[] { 11, 22, 33, 44, 55 }; }
        }

        public string Subject
        {
            get { return "Maths"; }
        }        

        public DateTimeOffset ValidFrom
        {
            get { return DateTimeOffset.MinValue; }
        }

        public DateTimeOffset ValidTo
        {
        get { return DateTimeOffset.MaxValue; }
    }

        public byte[] GetHashValue()
        {
            return new byte[] { 11, 22, 33, 44, 55 };
        }

        public byte[] GetHashValue(string hashAlgorithmName)
        {
            return new byte[] { 11, 22, 33, 44, 55 };
        }
    }

    public class FakeCertificateValidationViewModel : ICertificateValidationViewModel
    {
        public ICommand AcceptCertificateCommand { get { return null; } }

        public ICommand AcceptOnceCommand { get { return null; } }

        public ICommand CancelCommand { get { return null; } }

        public ICommand HideDetailsCommand { get { return null; } }

        public ICommand ShowDetailsCommand { get { return null; } }

        public IRdpCertificate Certificate
        {
            get
            {
                return new FakeRdpCertificate();
            }
        }

        public string Host
        {
            get
            {
                return "sample-Hostname";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsExpandedView
        {
            get
            {
                return true;
            }

            set
            {
                throw new NotImplementedException();
            }
        }


    }
}
