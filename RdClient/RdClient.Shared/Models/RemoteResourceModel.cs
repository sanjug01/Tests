namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteResourceModel : RemoteConnectionModel
    {
        private readonly string _resourceId;
        private readonly RemoteResourceType _resourceType;
        private readonly string _friendlyName;
        private readonly string _rdpFile;
        private readonly byte[] _iconBytes;
        private Guid _credId;

        public RemoteResourceModel(string strResourceId, RemoteResourceType resourceType, string strResourceFriendlyName, string strRdpFile, byte[] spIcon, uint iconWidth, Guid credId)
        {
            _resourceId = strResourceId;
            _resourceType = resourceType;
            _friendlyName = strResourceFriendlyName;
            _rdpFile = strRdpFile;
            _iconBytes = spIcon;
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
        }

        public RemoteResourceType ResourceType
        {
            get { return _resourceType; }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
        }

        public string RdpFile
        {
            get { return _rdpFile; }
        }

        public byte[] IconBytes
        {
            get { return _iconBytes; }
        }

        public override bool Equals(object obj)
        {
            RemoteResourceModel other = obj as RemoteResourceModel;
            return (other != null && this.ResourceId.Equals(other.ResourceId));
        }

        public override int GetHashCode()
        {
            return this.ResourceId.GetHashCode();
        }

        public override IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel)
        {

            IRdpConnection connection = null;
            if (this.ResourceType == RemoteResourceType.OnPremPublishedApp)
            {
               connection = connectionFactory.CreateApplication(this.RdpFile);                     
            }
            else if (this.ResourceType == RemoteResourceType.OnPremPublishedDesktop)
            {
                connection = connectionFactory.CreateDesktop(this.RdpFile);
            }
            else
            {
                throw new InvalidOperationException("Unexpected resource type");
            }
            return connection;
        }
    }
}
