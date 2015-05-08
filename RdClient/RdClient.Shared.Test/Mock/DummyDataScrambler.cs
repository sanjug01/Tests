namespace RdClient.Shared.Test.Mock
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using Windows.Security.Cryptography;
    using Windows.Storage.Streams;

    sealed class DummyDataScrambler : IDataScrambler
    {
        private static readonly string _salt = "salt:";

        ScrambledString IDataScrambler.Scramble(string value)
        {
            byte[] bytes;
            IBuffer utf = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);
            CryptographicBuffer.CopyToByteArray(utf, out bytes);
            return new ScrambledString(bytes, _salt);
        }

        string IDataScrambler.Unscramble(ScrambledString scrambledValue)
        {
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8,
                CryptographicBuffer.CreateFromByteArray(scrambledValue.Blob));
        }
    }

    [TestClass]
    public sealed class DummyDataScramblerTests
    {
        [TestMethod]
        public void DummyDataScrambler_ScrambleUnscrambleString_SameString()
        {
            const string value = "String Value";

            IDataScrambler scrambler = new DummyDataScrambler();

            Assert.AreEqual(value, scrambler.Unscramble(scrambler.Scramble(value)));
        }
    }
}
