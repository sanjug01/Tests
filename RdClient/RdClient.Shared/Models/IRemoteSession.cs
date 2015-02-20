namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;

    /// <summary>
    /// Interface of a remote session object that represents a session before the UI objects.
    /// </summary>
    public interface IRemoteSession
    {
        /// <summary>
        /// Certificate trust specific to this session.
        /// </summary>
        ICertificateTrust CertificateTrust { get; }

        /// <summary>
        /// Session emits the event when it needs user to enter credentials.
        /// </summary>
        event EventHandler<CredentialsNeededEventArgs> CredentialsNeeded;

        /// <summary>
        /// Session emits the event when it's got cancelled without ever being activated.
        /// </summary>
        event EventHandler Cancelled;

        /// <summary>
        /// Activate the remote session and attach a session view to it. The session becomes an exclusive
        /// owner of the view and may obtain a rendering panel from it.
        /// </summary>
        /// <param name="sessionView">Session view for which the session is activated.</param>
        /// <returns>Control object that may be used by the caller to send input to the remote session. The session
        /// may change the state of the returned control object. All changes are reported on the UI thread.</returns>
        IRemoteSessionControl Activate(IRemoteSessionView sessionView);

        /// <summary>
        /// Suspend the session. Once suspended, the session must release and recycle al its rendering panels.
        /// </summary>
        void Suspend();

        void Disconnect();
    }
}
