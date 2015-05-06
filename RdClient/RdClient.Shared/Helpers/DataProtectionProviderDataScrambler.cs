namespace RdClient.Shared.Helpers
{
    using System;
    using RdClient.Shared.Models;
    using Windows.Security.Cryptography.DataProtection;
    using System.Diagnostics.Contracts;
    using Windows.Security.Cryptography;
    using Windows.Storage.Streams;
    using System.Text;

    public sealed class DataProtectionProviderDataScrambler : IDataScrambler
    {
        private DataProtectionProvider _protector;

        public DataProtectionProviderDataScrambler()
        {
        }

        public string Scope
        {
            set
            {
                _protector = new DataProtectionProvider(value);
            }
        }

        ScrambledString IDataScrambler.Scramble(string value)
        {
            Contract.Assert(null != _protector);
            //
            // Generate a few random bytes and use them as salt for protecting the string - append the string
            // to salt and protect the resulting combined string. Return the protected bytes and salt in ScrambledString.
            //
            byte[] blob;
            IBuffer saltBuffer = CryptographicBuffer.GenerateRandom(4 + CryptographicBuffer.GenerateRandomNumber() % 8);
            StringBuilder sb = new StringBuilder(CryptographicBuffer.EncodeToBase64String(saltBuffer));
            string saltString = sb.ToString();
            sb.Append(value);
            IBuffer saltedStringBytes = CryptographicBuffer.ConvertStringToBinary(sb.ToString(), BinaryStringEncoding.Utf8);
            IBuffer protectedSaltedBytes = _protector.ProtectAsync(saltedStringBytes).AsTask().Result;
            CryptographicBuffer.CopyToByteArray(protectedSaltedBytes, out blob);
            return new ScrambledString(blob, saltString);
        }

        string IDataScrambler.Unscramble(ScrambledString scrambledValue)
        {
            Contract.Assert(null != _protector);
            Contract.Assert(null != scrambledValue);

            IBuffer scrambledBuffer = CryptographicBuffer.CreateFromByteArray(scrambledValue.Blob);
            IBuffer unscrambledBytes = _protector.UnprotectAsync(scrambledBuffer).AsTask().Result;
            string saltedString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, unscrambledBytes);

            if (!saltedString.StartsWith(scrambledValue.Salt, StringComparison.Ordinal))
                throw new ArgumentException("Bad salt", "scrambledValue");

            return saltedString.Substring(scrambledValue.Salt.Length);
        }
    }
}
