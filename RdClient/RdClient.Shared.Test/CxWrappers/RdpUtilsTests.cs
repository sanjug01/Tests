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
        public void TestApplyRdpProperties()
        {
            Desktop desktop = new Desktop() { hostName = "narf" };
            Mock.RdpProperties properties = new Mock.RdpProperties();

            RdpPropertyApplier.ApplyDesktop(properties, desktop);

            int hostNameIdx = properties._setStringPropertyName.IndexOf("Full Address");
            Assert.IsTrue(hostNameIdx >= 0);
            Assert.AreEqual("narf", properties._setStringPropertyValue[hostNameIdx]);
        }
    }
}
