using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public sealed class RdpCertificateToErrorListConverter : IValueConverter
    {
        private static readonly Dictionary<CertificateErrors, string> _codeMap;
        
        private IStringTable _localizedString;

        static RdpCertificateToErrorListConverter()
        {
            _codeMap = new Dictionary<CertificateErrors, string>();
            _codeMap[CertificateErrors.Expired] = "CertificateError_Expired_String";
            _codeMap[CertificateErrors.NameMismatch] = "CertificateError_NameMismatch_String";
            _codeMap[CertificateErrors.UntrustedRoot] = "CertificateError_UntrustedRoot_String";
            _codeMap[CertificateErrors.Revoked] = "CertificateError_Revoked_String";
            _codeMap[CertificateErrors.RevocationUnknown] = "CertificateError_RevocationUnknown_String";
            _codeMap[CertificateErrors.MismatchedCert] = "CertificateError_MismatchedCert_String";
            _codeMap[CertificateErrors.WrongEKU] = "CertificateError_WrongEKU_String";
            _codeMap[CertificateErrors.Critical] = "CertificateError_Critical_String";
        }

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IRdpCertificate cert = value as IRdpCertificate;
            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }     
            if (cert == null || cert.Error == null)
            {
                throw new ArgumentException("value to convert must be a non-null IRdpCertificate with a non-null Error property");
            }
            else
            {
                IList<string> errorList = new List<string>();
                foreach (CertificateErrors error in _codeMap.Keys)
                {
                    if (CertificateErrorHelper.ErrorContainsFlag(cert.Error.ErrorFlags, error))
                    {
                        errorList.Add(_localizedString.GetLocalizedString(_codeMap[error]));
                    }
                }
                return errorList;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
