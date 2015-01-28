namespace RdClient.Shared.Data
{
    using RdClient.Shared.CxWrappers;
    using System.Runtime.Serialization;

    /// <summary>
    /// A record of trust to an individual certificate.
    /// </summary>
    [DataContract(IsReference = true)]
    sealed class TrustedCertificate : SerializableModel
    {
        [DataMember(Name="Issuer")]
        private string _issuer;

        [DataMember(Name="SerialNumber")]
        private byte[] _serialNumber;

        [DataMember(Name = "Subject")]
        private string _subject;

        [DataMember(Name = "FriendlyName")]
        private string _friendlyName;

        public string Issuer { get { return _issuer; } }
        public byte[] SerialNumber { get { return _serialNumber; } }
        public string Subject { get { return _subject; } }
        public string FriendlyName { get { return _friendlyName; } }

        public TrustedCertificate(IRdpCertificate certificate)
        {
            _issuer = certificate.Issuer;
            _serialNumber = certificate.SerialNumber;
            _subject = certificate.Subject;
            _friendlyName = certificate.FriendlyName;
        }
    }
}
