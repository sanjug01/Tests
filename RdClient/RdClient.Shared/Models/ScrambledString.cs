namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Serializable combination of a scrambled BLOB and salt string.
    /// </summary>
    [DataContract]
    public sealed class ScrambledString
    {
        [DataMember(Name ="BLOB")]
        private readonly byte[] _blob;

        [DataMember(Name = "Salt")]
        private readonly string _salt;

        /// <summary>
        /// Construct a read-only ScrambledString object.
        /// </summary>
        /// <param name="blob">Encrypted salted data.</param>
        /// <param name="salt">Salt string added to the original string before encryption.</param>
        public ScrambledString(byte[] blob, string salt)
        {
            Contract.Assert(null != blob);
            Contract.Assert(!string.IsNullOrEmpty(salt));

            _blob = blob;
            _salt = salt;
        }

        /// <summary>
        /// Bytes of the encrypted combination of salt and scrambled strings.
        /// </summary>
        public byte[] Blob
        {
            get { return _blob; }
        }

        /// <summary>
        /// Random salt string added to the original string before scrambling.
        /// Adding salt makes scrambled blobs of thge same string different, which makes it impossible
        /// to find out that two scrambled strings are the same.
        /// After decrypting, the salt string is removed from the decrypted string.
        /// </summary>
        public string Salt
        {
            get { return _salt; }
        }
    }
}
