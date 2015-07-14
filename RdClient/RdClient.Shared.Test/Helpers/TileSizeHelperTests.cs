using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;
using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]
    public class TileSizeHelperTests
    {
        class TestWindowSize : IWindowSize
        {
            public TestWindowSize(Size size)
            {
                this.Size = size;
            }

            public Size Size { get; private set; }

        }


        [TestInitialize]
        public void TestSetup()
        {
        }



        [TestMethod]
        public void TestDynamicTileSizes_ForNarrowLayout()
        {
            Size windowSize;
            Size tileSize;

            // small screens
            // example1: 360x640
            windowSize = new Size(360, 640);
            tileSize = TileSizeHelper.GetTileSize(windowSize); 
            Assert.AreEqual(164.0, tileSize.Width);
            Assert.AreEqual(92.0, tileSize.Height);

            // example2: 320x569
            windowSize = new Size(320, 569);
            tileSize = TileSizeHelper.GetTileSize(windowSize);
            Assert.AreEqual(144.0, tileSize.Width);
            Assert.AreEqual(80.0, tileSize.Height);
        }

        [TestMethod]
        public void TestDynamicTileSizes_ForMediumScreens()
        {
            Size windowSize;
            Size tileSize;

            // large screens, max dimension below 1366
            // example3: 1024x640
            windowSize = new Size(1024, 640);
            tileSize = TileSizeHelper.GetTileSize(windowSize);
            Assert.AreEqual(236.0, tileSize.Width);
            Assert.AreEqual(132.0, tileSize.Height);
        }

        [TestMethod]
        public void TestDynamicTileSizes_ForLargeScreens()
        {
            Size windowSize;
            Size tileSize;

            // large screens, max dimension greater than 1365
            // example4: 1366x768
            windowSize = new Size(1366, 768);
            tileSize = TileSizeHelper.GetTileSize(windowSize);
            Assert.AreEqual(256.0, tileSize.Width);
            Assert.AreEqual(144.0, tileSize.Height);
        }

    }
}
