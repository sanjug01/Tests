using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// The MockBase class is a basic mocking framework which reduces the amount of typing required to implement mock classes.
//
// A mock class is a class which pretends to be a full implementation but instead just returns the values it is setup for.
//
// For example, let's say we have a class Foo. The method narf() in the class Bar interacts with methods in Foo.
// If we want to write a unit test for narf, we need to verify that the methods of the Foo instance contained in Bar have been called
// in the correct sequence with the correct parameters and returned the expected values.
//
// To implement a Mock class for Foo which verifies these things we:
//
// class MockFoo : IFoo, MockBase
// {
//      public int method(int param1, char param2)
//      {
//          return (int) Invoke(new object[] { param1, param2 };
//      }
// }
// 
//
// We are assuming here that IBar only has one method with the signature int method(int param1, char param2).
// Invoke is called on MockBase which through reflection gives information to the MockBase instance that the method has been called with param1 and param2
// as parameters.
//
// Note: make sure you add all parameters in the same order as in the signature to the array passed to Invoke.
//
// To use the class we would do something along the lines of:
// MockFoo mockFoo = new MockFoo();
// Bar bar = new Bar(mockFoo);
//
// mockFoo.Expect("method", new object[] { 3, 4 }, 5);
//
// bar.methodUnderTest();
//
//
// If methodUnderTest does anything with foo that's not rigged with an expect call, an exception is thrown and the test fails.
//
// This could be:
//  * methodUnderTest invokes a method on foo which we are not expecting
//  * methodUnderTest doesn't invoke all methods we expect it to invoke
//  * methodUnderTest invoked the method when we wanted it to but with parameters we didn't expect
//
// After we have verified that the method is indeed an expected invocation, the last parameter of Expect (here it is 5) is returned as a return value of foo.method().

namespace RdMock
{
    public class MockException : Exception
    {
        public MockException(string message)
            : base(message)
        {
        }
    }

    public struct MockCall
    {
        public string functionName;
        public IList<object> parameters;
    }

    public struct MockReturn
    {
        public string functionName;
        public object value;
    }

    public class MockBase
    {
        private IList<MockCall> _calls = new List<MockCall>();
        private IList<MockReturn> _returns = new List<MockReturn>();

        ~MockBase()
        {
            if(_calls.Count > 0)
            {
                throw new MockException("Expected " + _calls.Count + " more calls before exiting scope.");
            }
        }

        public MockBase Expect(string functionName_, IList<object> parameters_, object value_)
        {
            MockCall call = new MockCall() { functionName = functionName_, parameters = parameters_ };
            MockReturn retval = new MockReturn() { functionName = functionName_, value = value_ };

            _calls.Add(call);
            _returns.Add(retval);

            return this;
        }

        public object Invoke(object[] actualParameterValues)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            MethodBase method;
            int i;

            if(stackFrames.Count() < 2)
            {
                throw new MockException("Unexpected invocation context");
            }

            method = stackFrames[1].GetMethod();

            string methodName = method.Name;

            if(_calls[0].functionName.Equals(methodName) == false || _calls.Count < 1)
            {
                throw new MockException("Unexpected invocation of " + methodName);
            }

            MockCall call = _calls[0];
            _calls.RemoveAt(0);

            IList<object> expectedParameters = call.parameters;
            ParameterInfo[] actualParameters = method.GetParameters();

            if (expectedParameters.Count != actualParameters.Count())
            {
                throw new MockException("expected " + expectedParameters.Count + " parameters but invoked with " + actualParameters.Count() + " parameters.");
            }

            for (i = 0; i < actualParameters.Count(); i++)
            {
                Type expectedType = expectedParameters[i].GetType();
                Type actualType = actualParameters[i].ParameterType;

                if(expectedType.IsAssignableFrom(actualType) == false)
                {
                    throw new MockException("Parameter " + i + " is of type " + actualType.Name + " but expected type is " + expectedType.Name);
                }

                object expectedValue = expectedParameters[i];
                object actualValue = actualParameterValues[i];

                if (Convert.ChangeType(expectedValue, expectedType).Equals(Convert.ChangeType(actualValue, actualType)) == false)
                {
                    throw new MockException("Expected value: " + Convert.ChangeType(expectedValue, expectedType) + " Actual value: " + Convert.ChangeType(actualValue, actualType));
                }
            }

            if(_returns[0].functionName.Equals(methodName) == false || _returns.Count < 1)
            {
                throw new MockException("Initialized expectation but missing return value?!");
            }

            object retval = _returns[0].value;
            _returns.RemoveAt(0);

            return retval;
        }
    }
}
