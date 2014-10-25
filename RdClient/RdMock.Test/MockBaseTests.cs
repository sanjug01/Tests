using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RdMock.Test
{
    [TestClass]
    public class MockBaseTests
    {
        class TestMock : MockBase
        {

            public int testMethod(int a)
            {
                return (int) Invoke(new object[] {a});
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            bool exceptionThrown = false;
            try
            {
                {
                    TestMock tm = new TestMock();
                    List<object> pars = new List<object>() { 3 };

                    tm.Expect("testMethod", pars, 4);
                    int actual = tm.testMethod(3);
                    Assert.AreEqual(4, actual);
                }
            }
            catch(MockException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown);
        }
    }
}
