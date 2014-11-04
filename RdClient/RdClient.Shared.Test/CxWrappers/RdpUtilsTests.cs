﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Desktop desktop = new Desktop() { hostName = "narf" };

                properties.Expect("SetStringProperty", new List<object>() { "Full Address", "narf" }, 0);

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
