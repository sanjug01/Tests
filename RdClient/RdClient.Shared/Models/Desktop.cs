namespace RdClient.Shared.Models
{
    using System;
    using System.Runtime.Serialization;


    [DataContract(IsReference = true)]
    public sealed class Desktop : RemoteConnection
    {
        private string _hostName;
        private string _friendlyName;
        private bool _isUseAdminSession;
        private bool _isSwapMouseButtons;
        private AudioModes _audioMode;

        private Guid _credId;
        private Guid _thumbnailId;

        public enum AudioModes
        {
            Local = 0,
            Remote = 1,
            NoSound = 2
        }        

        public Desktop(Workspace parentWorkspace)
            : base(parentWorkspace)
        {
        }

        /// <summary>
        /// Default constructor for loading objects by a serializer.
        /// </summary>
        public Desktop()
        {
        }
        
        [DataMember]
        public string HostName
        {
            get { return _hostName; }
            set { SetProperty(ref _hostName, value);  }
        }

        [DataMember]
        public string FriendlyName
        {
            get { return _friendlyName; }
            set { SetProperty(ref _friendlyName, value); }
        }

        [DataMember]
        public bool IsUseAdminSession
        {
            get { return _isUseAdminSession; }
            set { SetProperty(ref _isUseAdminSession, value); }
        }

        [DataMember]
        public bool IsSwapMouseButtons
        {
            get { return _isSwapMouseButtons; }
            set { SetProperty(ref _isSwapMouseButtons, value); }
        }

        [DataMember]
        public AudioModes AudioMode
        {
            get { return _audioMode; }
            set { SetProperty(ref _audioMode, value); }
        }       

        [DataMember]
        public Guid CredentialId
        {            
            get { return _credId; }
            set { SetProperty(ref _credId, value); }
        }

        public bool HasCredential
        {
            get { return !this.CredentialId.Equals(Guid.Empty); }
        }

        [DataMember]
        public Guid ThumbnailId
        {
            get { return _thumbnailId; }
            set { SetProperty(ref _thumbnailId, value); }
        }

        public bool HasThumbnail
        {
            get { return !Guid.Empty.Equals(this.ThumbnailId); }
        }
    }
}
