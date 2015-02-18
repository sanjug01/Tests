namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSessionViewModel : ViewModelBase, IRemoteSessionViewSite
    {
        private IRemoteSessionView _sessionView;

        protected override void OnPresenting(object activationParameter)
        {
            base.OnPresenting(activationParameter);

            Contract.Assert(null != activationParameter, "Cannot navigate to remote session without activation parameter");
            Contract.Assert(activationParameter is IRemoteSession, "Remote session view model activation parameter is not IRemoteSession");
            //
            // TODO:    if the session view model had obtained the session view for the session, it may activate
            //          the session with that view. Otherwise it should just remember the session and wait for
            //          the view.
            //
            IRemoteSession newSession = (IRemoteSession)activationParameter;

            if (null != _sessionView)
            {
                newSession.Activate(_sessionView);
            }
        }

        protected override void OnDismissed()
        {
            base.OnDismissed();
        }

        void IRemoteSessionViewSite.SetRemoteSessionView(IRemoteSessionView sessionView)
        {
            _sessionView = sessionView;
        }
    }
}
