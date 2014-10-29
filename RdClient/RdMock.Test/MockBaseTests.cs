﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RdMock.Test
{
    [TestClass]
    public class MockBaseTests
    {
        class TestMock : MockBase
        {
            public int testMethod1(int a)
            {
                return (int) Invoke(new object[] {a});
            }

            public int testMethod2(int a, char b)
            {
                return (int)Invoke(new object[] { a, b });
            }

            public int testMethod3(int a)
            {
                return (int)Invoke(new object[] { a });
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

                    tm.Expect("testMethod1", pars, 4);
                    int actual = tm.testMethod1(3);
                    Assert.AreEqual(4, actual);
                }
            }
            catch(MockException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown);
        }

        [TestMethod]
        public void TestMethodSequence()
        {
            bool exceptionThrown = false;
            try
            {
                {
                    TestMock tm = new TestMock();

                    tm.Expect("testMethod1", new List<object>() { 3 }, 4)
                      .Expect("testMethod2", new List<object>() { 3, 'a' }, 5)
                      .Expect("testMethod3", new List<object>() { 23 }, 42);

                    int actual1 = tm.testMethod1(3);
                    int actual2 = tm.testMethod2(3, 'a');
                    int actual3 = tm.testMethod3(23);

                    Assert.AreEqual(4, actual1);
                    Assert.AreEqual(5, actual2);
                    Assert.AreEqual(42, actual3);
                }
            }
            catch (MockException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown);
        }
    }
}