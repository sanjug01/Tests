namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Modal dialog comlpletion object passed to the certificate validation view (CertificateValidationView)
    /// presented in a session.
    /// The object adds a failed certificate to one of the collections of trusted certificates and notifies
    /// the session about acceptance or rejection of the certificate by calling the validation object.
    /// </summary>
    public sealed class CertificateValidationCompletion : IPresentationCompletion
    {
            private readonly ICertificateValidation _validation;
            private readonly ICertificateTrust _permanentTrust, _sessionTrust;

            public CertificateValidationCompletion(ICertificateValidation validation, ICertificateTrust permanentTrust, ICertificateTrust sessionTrust)
            {
                Contract.Requires(null != validation);
                Contract.Assert(null != permanentTrust);
                Contract.Assert(null != sessionTrust);
                Contract.Ensures(null != _validation);
                Contract.Ensures(null != permanentTrust);
                Contract.Ensures(null != sessionTrust);

                _validation = validation;
                _permanentTrust = permanentTrust;
                _sessionTrust = sessionTrust;
            }

            void IPresentationCompletion.Completed(IPresentableView view, object result)
            {
                Contract.Assert(result is CertificateValidationResult);

                CertificateValidationResult r = (CertificateValidationResult)result;

                switch(r.Result)
                {
                    case CertificateValidationResult.CertificateTrustLevel.AcceptedAlways:
                        //
                        // Add the certificate to the persistent trusted certificates collection and allow the session to proceed.
                        //
                        _permanentTrust.TrustCertificate(_validation.Certificate);
                        _validation.Accept();
                        break;

                    case CertificateValidationResult.CertificateTrustLevel.AcceptedOnce:
                        //
                        // Add the certificate to the session's trusted certificates collection and allow the session to proceed.
                        //
                        _sessionTrust.TrustCertificate(_validation.Certificate);
                        _validation.Accept();
                        break;

                    default:
                        //
                        // Tell the session to disconnect.
                        //
                        _validation.Reject();
                        break;
                }
            }
    }
}
