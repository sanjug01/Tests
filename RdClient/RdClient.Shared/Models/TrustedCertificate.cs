namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public class TrustedCertificate : ModelBase
    {
        private byte[] _hash;
        private byte[] _serialNumber;

        [DataMember]
        public byte[] Hash
        {
            get { return _hash; }
            set { this.SetProperty<byte[]>(ref _hash, value); }
        }

        [DataMember]
        public byte[] SerialNumber
        {
            get { return _serialNumber; }
            set { this.SetProperty<byte[]>(ref _serialNumber, value); }
        }

        public TrustedCertificate(IRdpCertificate certificate)
        {
            Contract.Requires(null != certificate);
            Contract.Ensures(null != _hash);
            Contract.Ensures(null != _serialNumber);

            _hash = certificate.GetHashValue();
            Contract.Assert(null != _hash);
            _serialNumber = certificate.SerialNumber;
            Contract.Assert(null != _serialNumber);
        }

        public TrustedCertificate()
        {
            //
            // Default constructor required for serialization.
            //
        }

        public bool IsMatchingCertificate(IRdpCertificate certificate)
        {
            Contract.Requires(null != certificate);
            Contract.Assert(null != _hash);
            Contract.Assert(null != _serialNumber);
            return _serialNumber.SequenceEqual(certificate.SerialNumber)
                && _hash.SequenceEqual(certificate.GetHashValue());            
        }
    }
}
