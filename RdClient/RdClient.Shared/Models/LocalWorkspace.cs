namespace RdClient.Shared.Models
{
    /// <summary>
    /// THe class represents the local configuration - collection of manually configured remote desktop collections.
    /// </summary>
    public sealed class LocalWorkspace : Workspace
    {
        public LocalWorkspace(PersistentData persistentData) : base(persistentData)
        {
        }
    }
}
