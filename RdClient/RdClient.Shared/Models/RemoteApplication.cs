namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplication : RemoteResource
    {
        public RemoteApplication(Workspace workspace) : base(workspace)
        {
        }
    }
}
