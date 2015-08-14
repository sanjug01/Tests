namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Utils;
    using RdClient.Shared.Telemetry;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    [DefaultValue(Local)]
    public enum AudioMode : int
    {
        Local = 0,
        Remote = 1,
        NoSound = 2
    }        

    [DataContract(IsReference = true)]
    public sealed class DesktopModel : RemoteConnectionModel, ICredentialsIdModel
    {
        [DataMember(Name = "HostName", EmitDefaultValue = false)]
        private string _hostName;

        [DataMember(Name = "FriendlyName", EmitDefaultValue = false, IsRequired = false)]
        private string _friendlyName;

        [DataMember(Name = "CredentialsId", EmitDefaultValue = false)]
        private Guid _credentialsId;

        [DataMember(Name = "GatewayId", EmitDefaultValue = false)]
        private Guid _gatewayId;

        [DataMember(Name = "AdminSession", EmitDefaultValue = false)]
        private bool _isAdminSession;

        [DataMember(Name = "SwapMouseButtons", EmitDefaultValue = false)]
        private bool _isSwapMouseButtons;

        [DataMember(Name = "AudioMode", EmitDefaultValue = false)]
        private AudioMode _audioMode;

        [DataMember(Name = "IsTrusted", EmitDefaultValue = false)]
        private bool _isTrusted;

        [DataMember(Name = "IsNew", EmitDefaultValue = false)]
        private bool _isNew;

        public string HostName
        {
            get { return _hostName; }
            set
            {
                this.SetProperty(ref _hostName, value);
                // name change invalidates the trust
                this.IsTrusted = false;
                EmitPropertyChanged("DisplayName");
            }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set
            {
                this.SetProperty(ref _friendlyName, value);
                EmitPropertyChanged("DisplayName");
            }
        }

        public string DisplayName
        {
            get
            {
                if(string.IsNullOrEmpty(this.FriendlyName))
                {
                    return this.HostName;
                }
                return this.FriendlyName;
            }
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

        public Guid GatewayId
        {
            get { return _gatewayId; }
            set
            {
                if (this.SetProperty(ref _gatewayId, value))
                    EmitPropertyChanged("HasGateway");
            }
        }

        public bool HasGateway
        {
            get { return !Guid.Empty.Equals(_gatewayId); }
        }

        public bool IsAdminSession
        {
            get { return _isAdminSession; }
            set { this.SetProperty(ref _isAdminSession, value); }
        }

        public bool IsSwapMouseButtons
        {
            get { return _isSwapMouseButtons; }
            set { this.SetProperty(ref _isSwapMouseButtons, value); }
        }

        public AudioMode AudioMode
        {
            get { return _audioMode; }
            set { this.SetProperty(ref _audioMode, value); }
        }

        public bool IsTrusted
        {
            get { return _isTrusted; }
            set { this.SetProperty(ref _isTrusted, value); }
        }

        /// <summary>
        /// Indicator of a new desktop - one that has been added but has not yet been launched.
        /// </summary>
        public bool IsNew
        {
            get { return _isNew; }
            set { this.SetProperty(ref _isNew, value); }
        }

        public DesktopModel()
        {
            _credentialsId = Guid.Empty;
            _gatewayId = Guid.Empty;
        }

        public override IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel)
        {
            IRdpConnection connection = connectionFactory.CreateDesktop(string.Empty);
            IRdpProperties properties = connection as IRdpProperties;
            Contract.Assert(null != properties);
            RdpPropertyApplier.ApplyDesktop(properties, this);

            connection.SetLeftHandedMouseMode(this.IsSwapMouseButtons);

            //
            // Clear the "IsNew" flag when user connects to the remote desktop.
            //
            this.IsNew = false;

            return connection;
        }

        public override void InitializeSessionTelemetry(ApplicationDataModel dataModel, Telemetry.Events.SessionLaunch sessionLaunch)
        {
            sessionLaunch.sourceType = this.TelemetrySourceType;

            if (!Guid.Empty.Equals(_gatewayId))
            {
                GatewayModel gateway = dataModel.Gateways.GetModel(_gatewayId);
                sessionLaunch.gwyCreds = dataModel.GetCredentialsTelemetryTag(gateway.CredentialsId);
            }

            sessionLaunch.hostAddressType = GetHostAddressTypeTag(_hostName);
            sessionLaunch.hostCreds = dataModel.GetCredentialsTelemetryTag(_credentialsId);
        }

        protected override string GetTelemetrySourceType()
        {
            return Guid.Empty.Equals(_gatewayId) ? "localDesktop" : "localDesktopWithGateway";
        }
    }
}
