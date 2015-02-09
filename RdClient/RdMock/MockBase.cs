using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

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
// We are assuming here that IFoo only has one method with the signature int method(int param1, char param2).
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
// bar.MockFinalize();
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
    public class MockInvokedCall
    {
        public string FunctionName { get; private set; }
        public ParameterInfo[] ParameterInfo { get; private set; }
        public object[] ParameterValues { get; private set; }

        public MockInvokedCall(MethodBase method, object[] parameterValues)
        {
            this.FunctionName = method.Name;
            this.ParameterInfo = method.GetParameters();
            this.ParameterValues = parameterValues;
        }
    }

    public interface IMockCall
    {
        string FunctionName { get; }
        object VerifyInvokedCall(MockInvokedCall actualCall);
    }

    public class MockExpectedCall : IMockCall
    {
        private IList<object> _parameters;
        private object _returnValue;

        public MockExpectedCall(string functionName, IList<object> parameters, object returnValue)
        {
            this.FunctionName = functionName;
            _parameters = parameters;
            _returnValue = returnValue;
        }

        public string FunctionName { get; private set; }

        public object VerifyInvokedCall(MockInvokedCall actualCall)
        {
            IList<object> expectedParameters = _parameters;
            ParameterInfo[] actualParameters = actualCall.ParameterInfo;
            object[] actualParameterValues = actualCall.ParameterValues;

            if (expectedParameters.Count != actualParameters.Length)
            {
                throw new Exception(string.Format("Mock call to {0}() failed. Expected {1} parameters but invoked with {2}.",
                                                    this.FunctionName, expectedParameters.Count, actualParameters.Length));
            }

            for (int i = 0; i < actualParameters.Length; i++)
            {
                if (expectedParameters[i] == null)
                {
                    continue;
                }

                Type expectedType = expectedParameters[i].GetType();
                Type actualType = actualParameters[i].ParameterType;
                if (actualType.IsAssignableFrom(expectedType) == false)
                {
                    throw new Exception(string.Format("Mock call to {0}() failed. Parameter {1} is of type {2} but expected type is {3}",
                                                        this.FunctionName, i, actualType.Name, expectedType.Name));
                }

                object expectedValue = expectedParameters[i];
                object actualValue = actualParameterValues[i];
                if (actualValue.Equals(expectedValue) == false)
                {
                    throw new Exception(string.Format("Mock call to {0}() failed. Wrong value for parameter {1}. Expected value = {2}, Actual value = {3}",
                                                        this.FunctionName, i, expectedValue, actualValue));
                }
            }

            return _returnValue;
        }
    }

    public class MockCallback : IMockCall
    {
        private Func<object[], object> _mockCallback;
       
        public MockCallback(string functionName, Func<object[], object> mockCallback)
        {
            this.FunctionName = functionName;
            _mockCallback = mockCallback;
        }

        public string FunctionName { get; private set; }

        public object VerifyInvokedCall(MockInvokedCall actualCall)
        {
            if (_mockCallback != null)
            {
                return _mockCallback(actualCall.ParameterValues);
            }
            else
            {
                return null;
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
        Justification = "Intended for test")]
    public class MockBase : IDisposable
    {
        private Queue<IMockCall> _expectedCalls = new Queue<IMockCall>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "Intended for test"), 
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
            Justification = "Intended for test")]
        public void Dispose()
        {
            if (_expectedCalls.Count > 0)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("Some expected calls to a mock object did not occur. {0} expected {1} more calls before exiting scope. Missed calls = ", this.GetType(), _expectedCalls.Count);
                foreach (IMockCall call in _expectedCalls)
                {
                    msg.AppendFormat("{0}(...) ", call.FunctionName);
                }
                throw new Exception(msg.ToString());
            }
        }

        public MockBase Expect(string functionName, IList<object> parameters, object value)
        {
            MockExpectedCall call = new MockExpectedCall(functionName, parameters, value);
            _expectedCalls.Enqueue(call);
            return this;
        }

        public MockBase Expect(string functionName, Func<object[], object> mockCallback)
        {
            MockCallback call = new MockCallback(functionName, mockCallback);
            _expectedCalls.Enqueue(call);
            return this;
        }

        public object Invoke(object[] actualParameterValues)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            MockInvokedCall actualCall;
            IMockCall expectedCall;

            if (stackFrames.Count() < 2)
            {
                throw new Exception("Mock call failed. Invoke() called from an unexpected invocation context");
            }
            actualCall = new MockInvokedCall(stackFrames[1].GetMethod(), actualParameterValues);

            if (_expectedCalls.Count < 1)
            {
                throw new Exception("Mock call failed. Unexpected invocation of " + actualCall.FunctionName + "() when no calls were expected.");
            }
            expectedCall = _expectedCalls.Dequeue();

            if (!expectedCall.FunctionName.Equals(actualCall.FunctionName))
            {
                throw new Exception(string.Format("Mock call failed. Unexpected invocation of {0}() when expecting a call to {1}()",
                                                    actualCall.FunctionName, expectedCall.FunctionName));                    
            }
            return expectedCall.VerifyInvokedCall(actualCall);            
        }
    }
}
