namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Utils;
    using RdClient.Shared.Data;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class GatewayModel : SerializableModel
    {
        [DataMember(Name = "HostName", EmitDefaultValue = false)]
        private string _hostName;

        [DataMember(Name = "CredentialsId", EmitDefaultValue = false)]
        private Guid _credentialsId;

        public string HostName
        {
            get { return _hostName; }
            set { this.SetProperty(ref _hostName, value); }
        }

        public Guid CredentialsId
        {
            get { return _credentialsId; }
            set
            {
                if (this.SetProperty(ref _credentialsId, value))
                    EmitPropertyChanged("HasCredentials");
            }
        }

        public bool HasCredentials
        {
            get { return !Guid.Empty.Equals(_credentialsId); }
        }

        public GatewayModel()
        {
            _credentialsId = Guid.Empty;
        }

        public GatewayModel(GatewayModel otherModel)
        {
            Contract.Assert(null != otherModel);

            _hostName = otherModel.HostName;
            _credentialsId = otherModel.CredentialsId;
        }

        public void CopyTo(GatewayModel otherModel)
        {
            Contract.Requires(null != otherModel);
            otherModel.HostName = _hostName;
            otherModel.CredentialsId = _credentialsId;
        }

    }
}
