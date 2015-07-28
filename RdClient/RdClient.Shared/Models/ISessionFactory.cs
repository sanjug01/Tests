namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;

    /// <summary>
    /// Factory object that creates sessions.
    /// </summary>
    public interface ISessionFactory
    {
        /// <summary>
        /// Create an idle remote session for a connection object (either desktop or application)
        /// </summary>
        /// <param name="connection">Model object that deskcibes the connection.</param>
        /// <returns>Idle remote session object that may be passed down to the session view model for activation.</returns>
        /// <remarks>CreateSession is called when user clicks a connection tile in the connection center view; the created session is then passed
        /// down to the session view, that obtains a session view object from its view, and activates the session.</remarks>
        IRemoteSession CreateSession(RemoteSessionSetup sessionSetup);

    }
}
