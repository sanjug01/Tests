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
            using(Mock.RdpProperties properties = new Mock.RdpProperties())
            {
                DesktopModel desktop = new DesktopModel() { HostName = "narf" };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

                // default extra settings will be applied
                Assert.AreEqual(default(bool), desktop.IsAdminSession);
                Assert.AreEqual(default(AudioMode), desktop.AudioMode);

                properties.Expect("SetBoolProperty", new List<object>() { "Administrative Session", default(bool) }, 0);
                properties.Expect("SetIntProperty", new List<object>() { "AudioMode", (int)default(Desktop.AudioModes) }, 0);
                RdpPropertyApplier.ApplyDesktop(properties, desktop);
            }
        }

        [TestMethod]
        public void ApplyDesktopWithExtraSettings()
        {
            using (Mock.RdpProperties properties = new Mock.RdpProperties())
            {
                DesktopModel desktop = new DesktopModel() 
                { 
                    HostName = "narf" , 
                    IsSwapMouseButtons = true, 
                    FriendlyName = "MyPC",
                    IsAdminSession = true,
                    AudioMode = AudioMode.Remote
                };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

                // non-default extra settings - will be applied
                properties.Expect("SetBoolProperty", new List<object>() { "Administrative Session", true }, 0);
                properties.Expect("SetIntProperty", new List<object>() { "AudioMode", (int) AudioMode.Remote }, 0);

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
