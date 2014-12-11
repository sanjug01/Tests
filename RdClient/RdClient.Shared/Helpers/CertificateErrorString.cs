using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Helpers
{
    public class CertificateErrorString
    {
        private readonly IStringTable _localizedStrings;

        private Dictionary<CertificateErrors, string> codeMap = new Dictionary<CertificateErrors, string>();

        public CertificateErrorString(IStringTable localizedStrings)
        {
            _localizedStrings = localizedStrings;
                       
            codeMap[CertificateErrors.Expired] = "CertificateError_Expired_String";
            codeMap[CertificateErrors.NameMismatch] = "CertificateError_NameMismatch_String";
            codeMap[CertificateErrors.UntrustedRoot] = "CertificateError_UntrustedRoot_String";
            codeMap[CertificateErrors.Revoked] = "CertificateError_Revoked_String";
            codeMap[CertificateErrors.RevocationUnknown] = "CertificateError_RevocationUnknown_String";
            codeMap[CertificateErrors.MismatchedCert] = "CertificateError_MismatchedCert_String";
            codeMap[CertificateErrors.WrongEKU] = "CertificateError_WrongEKU_String";
            codeMap[CertificateErrors.Critical] = "CertificateError_Critical_String";
        }

        public string GetCertificateErrorString(CertificateErrors certificateError)
        {
            string localizedKey;
            string localizedValue;

            if (codeMap.ContainsKey(certificateError))
            {
                localizedKey = codeMap[certificateError];
            }
            else
            {
                localizedKey = "CertificateError_UnknownError_String";
            }

            localizedValue = _localizedStrings.GetLocalizedString(localizedKey);

            return localizedValue;
        }
    }
}
