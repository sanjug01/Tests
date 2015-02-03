namespace RdClient.Shared.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DefaultValue(Local)]
    public enum AudioMode : int
    {
        Local = 0,
        Remote = 1,
        NoSound = 2
    }        

    [DataContract(IsReference = true)]
    public sealed class DesktopModel : RemoteConnectionModel
    {
        [DataMember(Name = "HostName", EmitDefaultValue = false)]
        private string _hostName;

        [DataMember(Name = "FriendlyName", EmitDefaultValue = false, IsRequired = false)]
        private string _friendlyName;

        [DataMember(Name="CredentialsId")]
        private Guid _credentialsId;

        [DataMember(Name = "AdminSession", EmitDefaultValue = false)]
        private bool _isAdminSession;

        [DataMember(Name = "SwapMouseButtons", EmitDefaultValue = false)]
        private bool _isSwapMouseButtons;

        [DataMember(Name = "AudioMode", EmitDefaultValue = false)]
        private AudioMode _audioMode;

        public string HostName
        {
            get { return _hostName; }
            set { this.SetProperty(ref _hostName, value); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set { this.SetProperty(ref _friendlyName, value); }
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

        public DesktopModel()
        {
            _credentialsId = Guid.Empty;
        }
    }
}
