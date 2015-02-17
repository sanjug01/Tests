using RdClient.Shared.CxWrappers;
using System;

namespace RdClient.Shared.Helpers
{

    public static class CertificateErrorHelper
    {
        public static bool ErrorContainsFlag(CertificateError errorFlags, CertificateError error)
        {
            return (error & errorFlags) == error;
        }

        public static bool IsFatalCertificateError(IRdpCertificateError certError)
        {
            bool fIsFatal = WasCertErrorHitAndFatal(certError, CertificateError.Expired) ||
                            WasCertErrorHitAndFatal(certError, CertificateError.NameMismatch) ||
                            WasCertErrorHitAndFatal(certError, CertificateError.UntrustedRoot) ||
                            WasCertErrorHitAndFatal(certError, CertificateError.WrongEKU) ||
                            WasCertErrorHitAndFatal(certError, CertificateError.RevocationUnknown) ||
                            WasCertErrorHitAndFatal(certError, CertificateError.CertOrChainInvalid);

            if (!fIsFatal)
            {
                // If a certificate's revocation status cannot be determined, don't treat revocation as fatal
                if (!ErrorContainsFlag(certError.ErrorFlags, CertificateError.RevocationUnknown))
                {
                    fIsFatal = WasCertErrorHitAndFatal(certError, CertificateError.Revoked);
                }
            }

            return fIsFatal;
        }

        public static bool WasCertErrorHitAndFatal(IRdpCertificateError certError, CertificateError errorToCheck)
        {
            bool fIsFatal = false;

            if (ErrorContainsFlag(certError.ErrorFlags, errorToCheck))
            {
                // We hit the errorToCheck, so check if it's fatal
                if (ErrorContainsFlag(certError.ErrorFlags, CertificateError.Critical))
                {
                    fIsFatal = true;
                }
                else
                {
                    switch (errorToCheck)
                    {
                        case CertificateError.Revoked:
                        case CertificateError.MismatchedCert:
                            {
                                fIsFatal = true;
                                break;
                            }

                        case CertificateError.WrongEKU:
                            {
                                // Wrong EKU errors are only fatal if they come from CredSSP
                                fIsFatal = (ServerCertificateErrorSource.CredSSP == certError.ErrorSource);
                                break;
                            }
                    }
                }
            }

            return fIsFatal;
        }
    }
}
