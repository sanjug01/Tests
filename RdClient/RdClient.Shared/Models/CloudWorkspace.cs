namespace RdClient.Shared.Models
{
    /// <summary>
    /// Workspace deployed in the cloud.
    /// </summary>
    public sealed class CloudWorkspace : Workspace
    {
        public CloudWorkspace(RdDataModel persistentData)
            : base(persistentData)
        {
            //
            // TODO: set up the workspace properties.
            //
        }
    }
}
