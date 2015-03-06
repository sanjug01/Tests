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
        /// Host name or IP address of the remote system.
        /// </summary>
        string HostName { get; }

        /// <summary>
        /// State of the session synchronized with the UI thread.
        /// </summary>
        IRemoteSessionState State { get; }

        /// <summary>
        /// Certificate trust specific to this session.
        /// </summary>
        ICertificateTrust CertificateTrust { get; }

        /// <summary>
        /// Session emits the event when it needs user to enter credentials.
        /// </summary>
        event EventHandler<CredentialsNeededEventArgs> CredentialsNeeded;

        event EventHandler<BadCertificateEventArgs> BadCertificate;

        /// <summary>
        /// Session has failed permanently.
        /// </summary>
        event EventHandler<SessionFailureEventArgs> Failed;

        /// <summary>
        /// Connection has been interrupted and the session is trying to re-establish it.
        /// </summary>
        event EventHandler<SessionInterruptedEventArgs> Interrupted;

        /// <summary>
        /// Session has been permanently closed and cannot be re-connected.
        /// The session UI may be dismissed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Activate the remote session and attach a session view to it. The session becomes an exclusive
        /// owner of the view and may obtain a rendering panel from it.
        /// </summary>
        /// <param name="sessionView">Session view for which the session is activated.</param>
        /// <returns>Control object that may be used by the caller to send input to the remote session. The session
        /// may change the state of the returned control object. All changes are reported on the UI thread.</returns>
        /// <remarks>Upon activation the session may emit events alerting the UI about required intervention from a user.</remarks>
        IRemoteSessionControl Activate(IRemoteSessionView sessionView);

        /// <summary>
        /// Deactivate the session and detach its rendering panel.
        /// Deactivation means that the app navigates out of the view that renders the session,
        /// the session doesn't need to disconnect, just stop rendering.
        /// </summary>
        /// <returns></returns>
        IRenderingPanel Deactivate();

        /// <summary>
        /// Suspend the session. Once suspended, the session must release and recycle al its rendering panels.
        /// </summary>
        void Suspend();

        void Disconnect();
    }
}
