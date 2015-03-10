namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplicationModel : RemoteConnectionModel
    {
        public RemoteApplicationModel()
        {
        }

        public override IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel)
        {
            throw new System.NotImplementedException();
        }
    }
}
