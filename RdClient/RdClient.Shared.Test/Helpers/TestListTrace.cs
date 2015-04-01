using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]
    public class TestListTrace
    {
        [TestMethod]
        public void ListTrace_Rotates()
        {
            ListTrace<int> lt = new ListTrace<int>(3);

            lt.Add(1);
            lt.Add(2);
            lt.Add(3);
            lt.Add(4);

            int[] result = new int[3];
            lt.CopyTo(result, 0);

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(3, result[1]);
            Assert.AreEqual(4, result[2]);
        }
    }
}
