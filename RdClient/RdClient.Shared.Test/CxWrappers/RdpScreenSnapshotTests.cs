using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.Test.UAP;
using System;
using Windows.Graphics.Imaging;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpScreenSnapshotTests
    {
        TestData _testData = new TestData();

        [TestMethod]
        public void TestCreateWithNegativeWidthFails()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<OverflowException>(() =>
            {
                RdpScreenSnapshot snapshot = new RdpScreenSnapshot(-1, 20, new byte[1]);
            }));
        }

        [TestMethod]
        public void TestCreateWithNegativeHeightFails()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<OverflowException>(() =>
            {
                RdpScreenSnapshot snapshot = new RdpScreenSnapshot(200, -1, new byte[1]);
            }));
        }

        [TestMethod]
        public void TestCreateWithTooSmallBufferFails()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                int validWidth = _testData.RandomSource.Next(1, 20);
                int validHeight = _testData.RandomSource.Next(1, 20);
                int validBufferSize = validWidth * validHeight * RdpScreenSnapshot.BYTES_PER_PIXEL;
                RdpScreenSnapshot snapshot = new RdpScreenSnapshot(validWidth, validHeight, new byte[validBufferSize - 1]);
            }));
        }

        [TestMethod]
        public void TestCreateWithTooLargeBufferFails()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {

                int validWidth = _testData.RandomSource.Next(1, 20);
                int validHeight = _testData.RandomSource.Next(1, 20);
                int validBufferSize = validWidth * validHeight * RdpScreenSnapshot.BYTES_PER_PIXEL;
                RdpScreenSnapshot snapshot = new RdpScreenSnapshot(validWidth, validHeight, new byte[validBufferSize + 1]);
            }));
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
