namespace RdClient.Shared.Models
{
    /// <summary>
    /// Collection of desktops and applications deployed on premises.
    /// </summary>
    public sealed class OnPremiseWorkspace : Workspace
    {
        public OnPremiseWorkspace(RdDataModel persistentData)
            : base(persistentData)
        {
            //
            // TODO: set up the workspace properties
            //
        }
    }
}
