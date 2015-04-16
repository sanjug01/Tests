namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Modal dialog completion object passed to the validation view 
    /// presented in a session.
    /// The object adds a untrusted server to one of the collections of trusted servers and notifies
    /// the session about acceptance or rejection by calling the validation object.
    /// </summary>
    public sealed class DesktopIdentityValidationCompletion : IPresentationCompletion
    {
        private readonly IServerIdentityValidation _validation;
        private readonly IServerIdentityTrust _permanentTrust, _sessionTrust;

        public DesktopIdentityValidationCompletion(
            IServerIdentityValidation validation,
            IServerIdentityTrust permanentTrust,
            IServerIdentityTrust sessionTrust
            )
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
            Contract.Assert(result is DesktopIdentityValidationResult);

            DesktopIdentityValidationResult r = (DesktopIdentityValidationResult)result;

            switch (r.Result)
            {
                case DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedAlways:
                    // Add this host to the collection of trusted hosts and allow the session to proceed.
                    _permanentTrust.TrustServer(_validation.HostName);
                    _validation.Accept();
                    break;

                case DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedOnce:
                    // Add this host to to the session's trusted host collection and allow the session to proceed.
                    _sessionTrust.TrustServer(_validation.HostName);
                    _validation.Accept();
                    break;

                default:
                    // Tell the session to disconnect.
                    _validation.Reject();
                    break;
            }
        }
    }
}
