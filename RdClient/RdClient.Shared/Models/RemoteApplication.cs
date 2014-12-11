namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplication : RemoteResource
    {
        public RemoteApplication(Workspace parentWorkspace) : base(parentWorkspace)
        {
        }

        /// <summary>
        /// Default constructor for loading objects by a serializer.
        /// </summary>
        public RemoteApplication()
        {
        }
    }
}
