namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplicationModel : RemoteConnectionModel
    {
        private string _resourceId;
        private string _friendlyName;
        private string _rdpFile;
        private byte[] _iconBytes;
        private bool _hasIcon;

        public RemoteApplicationModel(string strResourceId, string strResourceFriendlyName, string strRdpFile, byte[] spIcon, uint iconWidth)
        {
            this.ResourceId = strResourceId;
            this.FriendlyName = strResourceFriendlyName;
            this.RdpFile = strRdpFile;
            this.IconBytes = spIcon;
        }

        public string ResourceId 
        {
            get { return _resourceId; }
            private set { SetProperty(ref _resourceId, value); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            private set { SetProperty(ref _friendlyName, value); }
        }

        public string RdpFile
        {
            get { return _rdpFile; }
            private set { SetProperty(ref _rdpFile, value); }
        }

        public byte[] IconBytes
        {
            get { return _iconBytes; }
            private set { SetProperty(ref _iconBytes, value); }
        }

        public override bool Equals(object obj)
        {
            RemoteApplicationModel other = obj as RemoteApplicationModel;
            return (other != null && this.ResourceId.Equals(other.ResourceId));
        }

        public override int GetHashCode()
        {
            return this.ResourceId.GetHashCode();
        }

        public override IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel)
        {
            IRdpConnection connection = connectionFactory.CreateApplication(this.RdpFile);                     
            return connection;
        }
    }
}
