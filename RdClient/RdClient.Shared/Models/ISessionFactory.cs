namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;

    /// <summary>
    /// Factory object that creates sessions.
    /// </summary>
    public interface ISessionFactory
    {
        /// <summary>
        /// Deferred execution object passed by the factory to sessions that it creates.
        /// Sessions use the deferred execution object to dispatch updates of their state
        /// to the UI thread.
        /// </summary>
        /// <remarks>The property must be set to a valid deferred execution object before
        /// the factory may create any sessions. The factory must throw an exception
        /// if it is told to create a session when it does not have a valid deferred execution
        /// object.</remarks>
        IDeferredExecution DeferedExecution { get; set; }

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
