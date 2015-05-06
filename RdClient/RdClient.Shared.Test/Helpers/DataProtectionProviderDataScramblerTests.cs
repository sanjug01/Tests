namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using Windows.Security.Cryptography;
    using Windows.Storage.Streams;

    [TestClass]
    public sealed class DataProtectionProviderDataScramblerTests
    {
        [TestMethod]
        public void DataProtectionProviderDataScrambler_Scramble_DataProduced()
        {
            IDataScrambler scrambler = new DataProtectionProviderDataScrambler() { Scope = "LOCAL=user" };
            ScrambledString ss = scrambler.Scramble("value");

            Assert.IsNotNull(ss.Blob);
            Assert.AreNotEqual(0, ss.Blob.Length);
            Assert.IsNotNull(ss.Salt);
            Assert.AreNotEqual(0, ss.Salt.Length);
        }

        [TestMethod]
        public void DataProtectionProviderDataScrambler_ScrambleSameString_DifferentBlobs()
        {
            IDataScrambler scrambler = new DataProtectionProviderDataScrambler() { Scope = "LOCAL=user" };
            ScrambledString
                ss1 = scrambler.Scramble("value"),
                ss2 = scrambler.Scramble("value");

            Assert.AreNotEqual(ss1.Salt, ss2.Salt);
            CollectionAssert.AreNotEqual(ss1.Blob, ss2.Blob);
        }

        [TestMethod]
        public void DataProtectionProviderDataScrambler_ScrambleUnscramble_Unscrambled()
        {
            IDataScrambler scrambler = new DataProtectionProviderDataScrambler() { Scope = "LOCAL=user" };
            ScrambledString ss = scrambler.Scramble("value");
            string unscrambled = scrambler.Unscramble(ss);

            Assert.AreEqual("value", unscrambled);
        }

        [TestMethod]
        public void DataProtectionProviderDataScrambler_BulkScrambleUnscramble_Unscrambled()
        {
            IDataScrambler scrambler = new DataProtectionProviderDataScrambler() { Scope = "LOCAL=user" };
            Random rnd = new Random();

            for (int i = 0; i < 500; ++i)
            {
                IBuffer randomBytes = CryptographicBuffer.GenerateRandom((uint)rnd.Next(5, 160));
                string value = CryptographicBuffer.EncodeToBase64String(randomBytes);
                ScrambledString ss = scrambler.Scramble(value);
                string unscrambled = scrambler.Unscramble(ss);

                Assert.AreEqual(value, unscrambled);
            }
        }
    }
}
