using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers.Errors
{
    public class RdpCertificateError : IRdpCertificateError, IRdpError
    {
        private RdClientCx.ServerCertificateError _serverCertificateError;
        public RdpCertificateError(RdClientCx.ServerCertificateError serverCertificateError)
        {
            _serverCertificateError = serverCertificateError;
        }

        public int ErrorCode
        {
            get { return _serverCertificateError.errorCode; }
        }

        public CertificateError ErrorFlags
        {
            get { return (CertificateError)_serverCertificateError.errorFlags; }
        }

        public ServerCertificateErrorSource ErrorSource
        {
            get { return (ServerCertificateErrorSource)_serverCertificateError.errorSource; }
        }
    }
}
