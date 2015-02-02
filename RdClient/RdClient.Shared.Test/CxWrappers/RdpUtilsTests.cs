using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System.Collections.Generic;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpUtilsTests
    {
        [TestMethod]
        public void ApplyDesktop()
        {
            RdDataModel data = new RdDataModel();

            using(Mock.RdpProperties properties = new Mock.RdpProperties())
            {
                Desktop desktop = new Desktop(data.LocalWorkspace) { HostName = "narf" };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

                // default extra settings will not be applied
                Assert.AreEqual(false, desktop.IsUseAdminSession);
                Assert.AreEqual(false, desktop.IsSwapMouseButtons);
                Assert.AreEqual(0, desktop.AudioMode);

                RdpPropertyApplier.ApplyDesktop(properties, desktop);
            }
        }

        [TestMethod]
        public void ApplyDesktopWithExtraSettings()
        {
            RdDataModel data = new RdDataModel();

            using (Mock.RdpProperties properties = new Mock.RdpProperties())
            {
                Desktop desktop = new Desktop(data.LocalWorkspace) 
                { 
                    HostName = "narf" , 
                    IsSwapMouseButtons = true, 
                    FriendlyName = "MyPC",
                    IsUseAdminSession = true,
                    AudioMode = 1
                };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

                // non-default extra settings - will be applied
                Assert.AreNotEqual(false, desktop.IsUseAdminSession);
                Assert.AreNotEqual(false, desktop.IsSwapMouseButtons);
                Assert.AreNotEqual(0, desktop.AudioMode);
                properties.Expect("SetBoolProperty", new List<object>() { "Administrative Session", true }, 0);
                properties.Expect("SetIntProperty", new List<object>() { "AudioMode", 1 }, 0);
                properties.Expect("SetLeftHandedMouseMode", new List<object>() { true }, 0);

                RdpPropertyApplier.ApplyDesktop(properties, desktop);
            }
        }

        [TestMethod]
        public void ApplyScreenSize()
        {
            ScreenSize size = new ScreenSize() { Width = 23, Height = 42 };

            using(Mock.RdpProperties properties = new Mock.RdpProperties())
            using (Mock.PhysicalScreenSize physicalSize = new Mock.PhysicalScreenSize(size))
            {
                physicalSize.Expect("GetScreenSize", new List<object>() { }, size);
                properties.Expect("SetIntProperty", new List<object>() { "PhysicalDesktopWidth", 23 }, 0);
                properties.Expect("SetIntProperty", new List<object>() { "PhysicalDesktopHeight", 42 }, 0);

                RdpPropertyApplier.ApplyScreenSize(properties, physicalSize);
            }
        }
    }
}
