namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using System.ComponentModel;

    [DefaultValue(Idle)]
    public enum SessionState
    {
        /// <summary>
        /// The session is idle - it is either waiting for some information from user
        /// before connecting, or hasn't been activated yet.
        /// </summary>
        Idle,
        /// <summary>
        /// The session is connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// The session is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// Connection has been interrupted and the session is trying to re-connect.
        /// </summary>
        Interrupted,
        /// <summary>
        /// The session has failed irecoverably.
        /// </summary>
        Failed,
        /// <summary>
        /// The session was closed by user.
        /// </summary>
        Closed
    }

    /// <summary>
    /// Representation of the state of a remote session synchronized with the UI thread.
    /// </summary>
    public interface IRemoteSessionState : INotifyPropertyChanged
    {
        SessionState State { get; }
        RdpDisconnectCode DisconnectCode { get; }
        int ReconnectAttempt { get; }
    }
}
