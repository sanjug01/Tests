namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Collection of desktops and applications deployed on premises.
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspace : Workspace
    {
        public OnPremiseWorkspace(RdDataModel persistentData)
            : base(persistentData)
        {
            //
            // TODO: set up the workspace properties
            //
        }

        public OnPremiseWorkspace()
        {
        }
    }
}
