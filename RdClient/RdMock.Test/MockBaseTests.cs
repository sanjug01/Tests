using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            public int testMethod4(object a)
            {
                return (int)Invoke(new object[] { a });
            }
        }

        [TestMethod]
        public void MockMethod1()
        {
            bool exceptionThrown = false;
            try
            {
                using (TestMock tm = new TestMock())
                {
                    List<object> pars = new List<object>() { 3 };

                    tm.Expect("testMethod1", pars, 4);
                    int actual = tm.testMethod1(3);
                    Assert.AreEqual(4, actual);
                }
            }
            catch(Exception /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown);
        }

        [TestMethod]
        public void MockMethodSequence()
        {
            using (TestMock tm = new TestMock())
            {
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

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MockRemainingExpectations()
        {
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod1", new List<object>() { 3 }, 4);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MockUnexpectedInvocation()
        {
            using (TestMock tm = new TestMock())
            {
                tm.testMethod1(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MockParameterCountNotMatching()
        {
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod1", new List<object>() { }, 4);
                tm.testMethod1(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MockParameterTypeNotMatching()
        {
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod1", new List<object>() { 'b' }, 4);
                tm.testMethod1(1);
            }
        }
        
        [TestMethod]
        public void MockParameterNull()
        {
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod4", new List<object>() { null }, 4);
                tm.testMethod4(5);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MockParameterValueNotMatching()
        {
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod1", new List<object>() { 5 }, 4);
                tm.testMethod1(1);
            }
        }

        [TestMethod]
        public void MockCallbackReceivesPassedParameters()
        {
            object[] passedParams = {3, 'a'};
            object[] receivedParams = null;
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod2",                    
                    p =>
                    { 
                        receivedParams = p; 
                        return 0; 
                    });
                tm.testMethod2((int)passedParams[0], (char)passedParams[1]);    
            }
            CollectionAssert.AreEqual(passedParams, receivedParams);
        }

        [TestMethod]
        public void CallToMockReturnsMockCallbackReturnValue()
        {
            int expectedReturn = 43;
            int actualReturn;
            using (TestMock tm = new TestMock())
            {
                tm.Expect("testMethod2",
                    p =>
                    {
                        return expectedReturn;
                    });
                actualReturn = tm.testMethod2(3, 'a');
            }
            Assert.AreEqual(expectedReturn, actualReturn);
        }
    }
}
