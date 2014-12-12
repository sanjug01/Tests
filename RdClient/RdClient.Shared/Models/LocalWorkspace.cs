namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// THe class represents the local configuration - collection of manually configured remote desktop collections.
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class LocalWorkspace : Workspace
    {
        public LocalWorkspace(RdDataModel dataModel) : base(dataModel)
        {
        }

        public LocalWorkspace()
        {
        }
    }
}
