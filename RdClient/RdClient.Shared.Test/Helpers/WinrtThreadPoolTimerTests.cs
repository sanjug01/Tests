﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]
    public class WinrtThreadPoolTimerTests : ITimerTests
    {
        public override ITimerFactory GetFactory()
        {
            return new WinrtThreadPoolTimerFactory();
        }
    }
}