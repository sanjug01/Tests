namespace RdClient.Shared.Helpers
{
    using System;
    using RdClient.Shared.Models;
    using Windows.Security.Cryptography.DataProtection;
    using System.Diagnostics.Contracts;
    using Windows.Security.Cryptography;
    using Windows.Storage.Streams;
    using System.Text;
    using Windows.Security.Credentials;

    public sealed class DataProtectionProviderDataScrambler : IDataScrambler
    {
        private DataProtectionProvider _protector;
        //
        // Application's private key stored in the password vault. The class creates the random private key once
        // and saves it in the password vault with resource name "PrivateKey" and user name "Application".
        //
        private string _privateKey;

        public DataProtectionProviderDataScrambler()
        {
        }

        public string Scope
        {
            set
            {
                _protector = new DataProtectionProvider(value);
                _privateKey = RetrieveOrCreatePrivateKey();
            }
        }

        ScrambledString IDataScrambler.Scramble(string value)
        {
            Contract.Assert(null != _protector);
            Contract.Assert(!string.IsNullOrEmpty(_privateKey));
            //
            // Generate a few random bytes and use them as salt for protecting the string - append the string
            // to salt and protect the resulting combined string. Return the protected bytes and salt in ScrambledString.
            //
            // After the salt add the private key stored in the password vault.
            //
            byte[] blob;
            IBuffer saltBuffer = CryptographicBuffer.GenerateRandom(4 + CryptographicBuffer.GenerateRandomNumber() % 8);
            StringBuilder sb = new StringBuilder(CryptographicBuffer.EncodeToBase64String(saltBuffer));
            string saltString = sb.ToString();
            sb.Append(_privateKey);
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
            string saltPart = scrambledValue.Salt + _privateKey;

            if (!saltedString.StartsWith(saltPart, StringComparison.Ordinal))
                throw new ArgumentException("Bad salt", "scrambledValue");

            return saltedString.Substring(saltPart.Length);
        }

        private string RetrieveOrCreatePrivateKey()
        {
            const string
                Resource = "PrivateKey",
                UserName = "Application";

            PasswordVault vault = new PasswordVault();
            PasswordCredential cred;

            try
            {
                cred = vault.Retrieve(Resource, UserName);
            }
            catch
            {
                //
                // Gererate 8 random bytes of the private key and stash the key in the vault.
                //
                cred = new PasswordCredential(Resource, UserName,
                    CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(8)));
                vault.Add(cred);
            }

            return cred.Password;
        }
    }
}
