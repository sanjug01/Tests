namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Base class for all specific workspace classes - local workspace, on-premise workspace and Azure workspace.
    /// </summary>
    public abstract class Workspace : ModelBase
    {
        private readonly PersistentData _persistentData;
        private readonly ModelCollection<RemoteResource> _connections;

        protected Workspace(PersistentData persistentData)
        {
            Contract.Requires(null != persistentData);
            Contract.Ensures(null != _persistentData);
            Contract.Ensures(null != _connections);

            _persistentData = persistentData;
            _connections = new ModelCollection<RemoteResource>();
        }

        public ModelCollection<RemoteResource> Connections
        {
            get { return _connections; }
        }
    }
}
