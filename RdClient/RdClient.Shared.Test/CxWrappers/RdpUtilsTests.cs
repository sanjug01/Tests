using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpUtilsTests
    {
        [TestMethod]
        public void TestApplyDesktop()
        {
            Desktop desktop = new Desktop() { hostName = "narf" };
            Mock.RdpProperties properties = new Mock.RdpProperties();

            RdpPropertyApplier.ApplyDesktop(properties, desktop);

            int hostNameIdx = properties._setStringPropertyName.IndexOf("Full Address");
            Assert.IsTrue(hostNameIdx >= 0);
            Assert.AreEqual("narf", properties._setStringPropertyValue[hostNameIdx]);
        }

        public void TestApplyScreenSize()
        {
            Tuple<int, int> size = new Tuple<int, int>(23, 42);
            Mock.RdpProperties properties = new Mock.RdpProperties();
            Mock.PhysicalScreenSize physicalSize = new Mock.PhysicalScreenSize(size);

            RdpPropertyApplier.ApplyScreenSize(properties, physicalSize);

            int widthIdx = properties._setStringPropertyName.IndexOf("PhysicalDesktopWidth");
            Assert.IsTrue(widthIdx >= 0);
            Assert.AreEqual(23, properties._setStringPropertyValue[widthIdx]);

            int heightIdx = properties._setStringPropertyName.IndexOf("PhysicalDesktopHeight");
            Assert.IsTrue(heightIdx >= 0);
            Assert.AreEqual(42, properties._setStringPropertyValue[heightIdx]);
        }
    }
}
