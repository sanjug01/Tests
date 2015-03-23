namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplicationModel : RemoteConnectionModel
    {
        private string _resourceId;
        private string _friendlyName;
        private string _rdpFile;
        private byte[] _iconBytes;
        private bool _hasIcon;
        private Guid _credId;

        public RemoteApplicationModel(string strResourceId, string strResourceFriendlyName, string strRdpFile, byte[] spIcon, uint iconWidth, Guid credId)
        {
            this.ResourceId = strResourceId;
            this.FriendlyName = strResourceFriendlyName;
            this.RdpFile = strRdpFile;
            this.IconBytes = spIcon;
            this.CredentialId = credId;
        }

        public Guid CredentialId
        {
            get { return _credId; }
            set { SetProperty(ref _credId, value); }
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
            get 
            { 
                return _iconBytes; 
            }
            private set 
            { 
                SetProperty(ref _iconBytes, value);
                this.HasIcon = value != null;
            }
        }

        public bool HasIcon
        {
            get { return _hasIcon; }
            private set { SetProperty(ref _hasIcon, value); }
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
