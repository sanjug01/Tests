namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public sealed class LocalWorkspaceModel : SerializableModel, IEquatable<LocalWorkspaceModel>
    {
        bool IEquatable<LocalWorkspaceModel>.Equals(LocalWorkspaceModel other)
        {
            return null != other;
        }
    }
}
