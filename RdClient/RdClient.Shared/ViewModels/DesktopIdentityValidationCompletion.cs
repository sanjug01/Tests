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

        public DesktopIdentityValidationCompletion(
            IServerIdentityValidation validation
            )
        {
            Contract.Requires(null != validation);
            Contract.Ensures(null != _validation);

            _validation = validation;
        }

        void IPresentationCompletion.Completed(IPresentableView view, object result)
        {
            Contract.Assert(result is DesktopIdentityValidationResult);

            DesktopIdentityValidationResult r = (DesktopIdentityValidationResult)result;

            switch (r.Result)
            {
                case DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedAlways:
                    // Accept and save to desktop.
                    _validation.Desktop.IsTrusted = true;
                    _validation.Accept();
                    break;

                case DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedOnce:
                    // Accept without persistence.
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
