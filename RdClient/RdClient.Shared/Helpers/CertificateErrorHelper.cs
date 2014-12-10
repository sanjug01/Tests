using RdClient.Shared.CxWrappers;
using System;

namespace RdClient.Shared.Helpers
{

    public static class CertificateErrorHelper
    {
        public static bool ErrorContainsFlag(CertificateErrors errorFlags, CertificateErrors error)
        {
            return (error & errorFlags) == error;
        }

        public static bool IsFatalCertificateError(IRdpCertificateError certError)
        {
            bool fIsFatal = WasCertErrorHitAndFatal(certError, CertificateErrors.Expired) ||
                            WasCertErrorHitAndFatal(certError, CertificateErrors.NameMismatch) ||
                            WasCertErrorHitAndFatal(certError, CertificateErrors.UntrustedRoot) ||
                            WasCertErrorHitAndFatal(certError, CertificateErrors.WrongEKU) ||
                            WasCertErrorHitAndFatal(certError, CertificateErrors.RevocationUnknown) ||
                            WasCertErrorHitAndFatal(certError, CertificateErrors.CertOrChainInvalid);

            if (!fIsFatal)
            {
                // If a certificate's revocation status cannot be determined, don't treat revocation as fatal
                if (!ErrorContainsFlag(certError.ErrorFlags, CertificateErrors.RevocationUnknown))
                {
                    fIsFatal = WasCertErrorHitAndFatal(certError, CertificateErrors.Revoked);
                }
            }

            return fIsFatal;
        }

        public static bool WasCertErrorHitAndFatal(IRdpCertificateError certError, CertificateErrors errorToCheck)
        {
            bool fIsFatal = false;

            if (ErrorContainsFlag(certError.ErrorFlags, errorToCheck))
            {
                // We hit the errorToCheck, so check if it's fatal
                if (ErrorContainsFlag(certError.ErrorFlags, CertificateErrors.Critical))
                {
                    fIsFatal = true;
                }
                else
                {
                    switch (errorToCheck)
                    {
                        case CertificateErrors.Revoked:
                        case CertificateErrors.MismatchedCert:
                            {
                                fIsFatal = true;
                                break;
                            }

                        case CertificateErrors.WrongEKU:
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
