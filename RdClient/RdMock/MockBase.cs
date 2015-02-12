using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                throw new MockException(msg.ToString());
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

        private MethodInfo GetMethodInfo(string methodName)
        {
            Type type = this.GetType();
            TypeInfo typeInfo = type.GetTypeInfo();

            IList<Type> types = new List<Type>();

            types.Add(type);
            foreach(Type t in typeInfo.ImplementedInterfaces)
            {
                types.Add(t);
            }

            foreach(Type t in types)
            {
                foreach(MethodInfo m in t.GetTypeInfo().DeclaredMethods)
                {
                    if(m.Name.Split('.').Last() == methodName)
                    {
                        return m;
                    }
                }
            }

            return null;
        }

        public object Invoke(object[] actualParameterValues, [CallerMemberName] string callerMemberName = "")
        {
            MockInvokedCall actualCall;
            IMockCall expectedCall;

            MethodInfo method = GetMethodInfo(callerMemberName);

            actualCall = new MockInvokedCall(method, actualParameterValues);

            if(actualCall == null)
            {
                throw new MockException(string.Format("Mock call failed. Trying to call {0}() which is not declared in the current mock.",
                                                    callerMemberName));
            }

            if (_expectedCalls.Count < 1)
            {
                throw new MockException("Mock call failed. Unexpected invocation of " + actualCall.FunctionName + "() when no calls were expected.");
            }
            expectedCall = _expectedCalls.Dequeue();
            //
            // Split the calling method name because for explicit interface implementations the namespace and class
            // are included in the calling function name.
            //
            if (!expectedCall.FunctionName.Equals(actualCall.FunctionName.Split('.').Last()))
            {
                throw new MockException(string.Format("Mock call failed. Unexpected invocation of {0}() when expecting a call to {1}()",
                                                    actualCall.FunctionName, expectedCall.FunctionName));                    
            }
            return expectedCall.VerifyInvokedCall(actualCall);            
        }
    }
}
