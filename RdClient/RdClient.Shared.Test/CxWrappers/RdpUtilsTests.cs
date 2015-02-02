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
                Assert.AreEqual(Desktop.AudioModes.Local, desktop.AudioMode);

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
                    AudioMode = Desktop.AudioModes.Remote
                };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

                // non-default extra settings - will be applied
                Assert.AreNotEqual(false, desktop.IsUseAdminSession);
                Assert.AreNotEqual(false, desktop.IsSwapMouseButtons);
                Assert.AreNotEqual(Desktop.AudioModes.Local, desktop.AudioMode);
                properties.Expect("SetBoolProperty", new List<object>() { "Administrative Session", true }, 0);
                properties.Expect("SetIntProperty", new List<object>() { "AudioMode", (int) Desktop.AudioModes.Remote }, 0);
                properties.Expect("SetLeftHandedMouseModeProperty", new List<object>() { true }, 0);

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
