﻿using RdClient.Shared.Helpers;
namespace RdClient.Shared.Models
{
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
    }
}