using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public sealed class CertificateErrorLocalizer : IValueConverter
    {
        private static readonly Dictionary<CertificateErrors, string> _codeMap;
        private const string DEFAULT_ID = "CertificateError_UnknownError_String";

        static CertificateErrorLocalizer()
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

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            CertificateErrors error = (CertificateErrors) value;
            string resourceId;

            if (!_codeMap.TryGetValue(error, out resourceId))
            {
                resourceId = DEFAULT_ID;
            }
            return resourceId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
