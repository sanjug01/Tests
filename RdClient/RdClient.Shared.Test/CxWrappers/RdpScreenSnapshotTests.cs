using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Test.Helpers;
using System;
using Windows.Graphics.Imaging;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpScreenSnapshotTests
    {
        TestData _testData = new TestData();

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void TestCreateWithNegativeWidthFails()
        {
            RdpScreenSnapshot snapshot = new RdpScreenSnapshot(-1, 20, new byte[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void TestCreateWithNegativeHeightFails()
        {
            RdpScreenSnapshot snapshot = new RdpScreenSnapshot(200, -1, new byte[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateWithTooSmallBufferFails()
        {
            int validWidth = _testData.RandomSource.Next(1, 20);
            int validHeight = _testData.RandomSource.Next(1, 20);
            int validBufferSize = validWidth * validHeight * RdpScreenSnapshot.BYTES_PER_PIXEL;
            RdpScreenSnapshot snapshot = new RdpScreenSnapshot(validWidth, validHeight, new byte[validBufferSize - 1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateWithTooLargeBufferFails()
        {
            int validWidth = _testData.RandomSource.Next(1, 20);
            int validHeight = _testData.RandomSource.Next(1, 20);
            int validBufferSize = validWidth * validHeight * RdpScreenSnapshot.BYTES_PER_PIXEL;
            RdpScreenSnapshot snapshot = new RdpScreenSnapshot(validWidth, validHeight, new byte[validBufferSize + 1]);
        }

        [TestMethod]
        public void TestCreateWithValidParametersWorks()
        {
            int validWidth = _testData.RandomSource.Next(1, 20);
            int validHeight = _testData.RandomSource.Next(1, 20);
            int validBufferSize = validWidth * validHeight * RdpScreenSnapshot.BYTES_PER_PIXEL;
            byte[] buffer = new byte[validBufferSize];
            _testData.RandomSource.NextBytes(buffer);
            RdpScreenSnapshot snapshot = new RdpScreenSnapshot(validWidth, validHeight, buffer);
            Assert.IsTrue(validHeight == snapshot.Height);
            Assert.IsTrue(validWidth == snapshot.Width);
            Assert.AreEqual(buffer, snapshot.RawImage);
            Assert.AreNotEqual(BitmapPixelFormat.Unknown, snapshot.PixelFormat);
        }
    }
}
