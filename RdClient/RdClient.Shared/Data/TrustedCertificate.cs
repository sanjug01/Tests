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

        public string Issuer { get { return _issuer; } }
        public byte[] SerialNumber { get { return _serialNumber; } }

        public TrustedCertificate(IRdpCertificate certificate)
        {
            _issuer = certificate.Issuer;
            _serialNumber = certificate.SerialNumber;
        }
    }
}
