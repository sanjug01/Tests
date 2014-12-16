namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Workspace deployed in the cloud.
    /// </summary>
    [DataContract(IsReference=true)]
    public sealed class CloudWorkspace : Workspace
    {
        public CloudWorkspace(RdDataModel persistentData)
            : base(persistentData)
        {
            //
            // TODO: set up the workspace properties.
            //
        }

        public CloudWorkspace()
        {
        }
    }
}
