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

        TileSizeHelper _tileSizeHelper;
        Size _windowSize;

        [TestInitialize]
        public void TestSetup()
        {
            _tileSizeHelper = new TileSizeHelper();
        }



        [TestMethod]
        public void TestDynamicTileSizes_ForNarrowLayout()
        {
            // small screens
            // example1: 360x640
            _windowSize = new Size(360, 640);
            _tileSizeHelper.WindowSize = new TestWindowSize(_windowSize);
            Assert.AreEqual(164.0, _tileSizeHelper.TileSize.Width);
            Assert.AreEqual(92.0, _tileSizeHelper.TileSize.Height);

            // example2: 320x569
            _windowSize = new Size(320, 569);
            _tileSizeHelper.WindowSize = new TestWindowSize(_windowSize);
            Assert.AreEqual(144.0, _tileSizeHelper.TileSize.Width);
            Assert.AreEqual(80.0, _tileSizeHelper.TileSize.Height);
        }

        [TestMethod]
        public void TestDynamicTileSizes_ForMediumScreens()
        {
            // large screens, max dimension below 1366
            // example3: 1024x640
            _windowSize = new Size(1024, 640);
            _tileSizeHelper.WindowSize = new TestWindowSize(_windowSize);
            Assert.AreEqual(236.0, _tileSizeHelper.TileSize.Width);
            Assert.AreEqual(132.0, _tileSizeHelper.TileSize.Height);
        }

        [TestMethod]
        public void TestDynamicTileSizes_ForLargeScreens()
        {
            // large screens, max dimension greater than 1365
            // example4: 1366x768
            _windowSize = new Size(1366, 768);
            _tileSizeHelper.WindowSize = new TestWindowSize(_windowSize);
            Assert.AreEqual(256.0, _tileSizeHelper.TileSize.Width);
            Assert.AreEqual(144.0, _tileSizeHelper.TileSize.Height);
        }

    }
}
