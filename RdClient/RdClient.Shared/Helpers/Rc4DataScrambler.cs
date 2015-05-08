namespace RdClient.Shared.Helpers
{
    using RdClient.Shared.Models;
    using System;
    using System.Text;
    using Windows.Security.Credentials;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;

    public sealed class Rc4DataScrambler : IDataScrambler
    {
        private readonly SymmetricKeyAlgorithmProvider _algorithm;
        //
        // Application's private key stored in the password vault. The class creates the random private key once
        // and saves it in the password vault with resource name "PrivateKey" and user name "RC4".
        //
        private readonly IBuffer _privateKey;

        public Rc4DataScrambler()
        {
            _algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.Rc4);
            _privateKey = RetrieveOrCreatePrivateKey();
        }

        ScrambledString IDataScrambler.Scramble(string value)
        {
            byte[] blob;
            string salt = MakeSaltString();
            StringBuilder sb = new StringBuilder(salt);
            sb.Append(value);
            IBuffer saltedStringBytes = CryptographicBuffer.ConvertStringToBinary(sb.ToString(), BinaryStringEncoding.Utf8);
            IBuffer protectedSaltedBytes = EncryptString(sb.ToString(), _privateKey);
            CryptographicBuffer.CopyToByteArray(protectedSaltedBytes, out blob);
            return new ScrambledString(blob, salt);
        }

        string IDataScrambler.Unscramble(ScrambledString scrambledValue)
        {
            string saltedString = DecryptString(CryptographicBuffer.CreateFromByteArray(scrambledValue.Blob), _privateKey);

            if (!saltedString.StartsWith(scrambledValue.Salt, StringComparison.Ordinal))
                throw new ArgumentException("Bad salt", "scrambledValue");
            //
            // Remove salt and return the decrypted value.
            //
            return saltedString.Substring(scrambledValue.Salt.Length);
        }

        private IBuffer EncryptString(string message, IBuffer keyMaterial)
        {
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8);
            return CryptographicEngine.Encrypt(_algorithm.CreateSymmetricKey(keyMaterial), buffMsg, null);
        }

        public string DecryptString(IBuffer encryptedData, IBuffer keyMaterial)
        {
            IBuffer buffDecrypted = CryptographicEngine.Decrypt(_algorithm.CreateSymmetricKey(keyMaterial), encryptedData, null);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
        }

        private static IBuffer RetrieveOrCreatePrivateKey()
        {
            const string
                Resource = "PrivateKey",
                UserName = "RC4";

            PasswordVault vault = new PasswordVault();
            IBuffer privateKey;

            try
            {
                PasswordCredential cred = vault.Retrieve(Resource, UserName);
                privateKey = CryptographicBuffer.DecodeFromBase64String(cred.Password);
            }
            catch
            {
                //
                // Gererate 16 random bytes of the private key and stash the key in the vault.
                //
                privateKey = CryptographicBuffer.GenerateRandom(16);
                vault.Add(new PasswordCredential(Resource, UserName, CryptographicBuffer.EncodeToBase64String(privateKey)));
            }

            return privateKey;
        }

        private static string MakeSaltString()
        {
            IBuffer saltBuffer = CryptographicBuffer.GenerateRandom(4 + CryptographicBuffer.GenerateRandomNumber() % 8);
            return CryptographicBuffer.EncodeToBase64String(saltBuffer);
        }
    }
}
