namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class CloudWorkspaceModel : SerializableModel, IEquatable<CloudWorkspaceModel>
    {
        bool IEquatable<CloudWorkspaceModel>.Equals(CloudWorkspaceModel other)
        {
            return null != other;
        }
    }
}
