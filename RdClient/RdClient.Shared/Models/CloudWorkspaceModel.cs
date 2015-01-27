namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class CloudWorkspaceModel : SerializableModel
    {
    }
}
