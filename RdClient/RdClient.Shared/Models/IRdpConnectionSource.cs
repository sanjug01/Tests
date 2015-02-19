namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;

    /// <summary>
    /// Interface of an object that creates RDP sessions bound to a specific rendering panel.
    /// The interface is used to inject the existing RDP connection factory that attaches the same
    /// swap chain panel to all connections in the session object model.
    /// </summary>
    /// <remarks>The application creates one instance of the connection source and uses a navigation extension
    /// to inject the instance into view models.</remarks>
    public interface IRdpConnectionSource
    {
        IRdpConnection CreateConnection(IRenderingPanel renderingPanel);
    }
}
