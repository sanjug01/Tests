using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

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
        private IDictionary<string, IList<MockCall>> _calls = new Dictionary<string, IList<MockCall>>();
        private IDictionary<string, IList<MockReturn>> _returns = new Dictionary<string, IList<MockReturn>>();

        ~MockBase()
        {
            foreach (string functionName in _calls.Keys)
            {
                if (_calls[functionName].Count > 0)
                {
                    throw new MockException("Expecing " + functionName + " to be called " + _calls[functionName].Count + " more times.");
                }
            }
        }

        public MockBase Expect(string functionName_, IList<object> parameters_, object value_)
        {
            MockCall call = new MockCall() { functionName = functionName_, parameters = parameters_ };
            MockReturn retval = new MockReturn() { functionName = functionName_, value = value_ };

            if (_calls.ContainsKey(functionName_) == false)
            {
                _calls[functionName_] = new List<MockCall>();
            }

            _calls[functionName_].Add(call);

            if (_returns.ContainsKey(functionName_) == false)
            {
                _returns[functionName_] = new List<MockReturn>();
            }

            _returns[functionName_].Add(retval);

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

            if(_calls.ContainsKey(methodName) == false || _calls[methodName].Count() < 1)
            {
                throw new MockException("Unexpected invocation of " + methodName);
            }

            MockCall call = _calls[methodName][0];
            _calls[methodName].RemoveAt(0);

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

            if(_returns.ContainsKey(methodName) == false || _returns[methodName].Count < 1)
            {
                throw new MockException("Initialized expectation but missing return value?!");
            }

            object retval = _returns[methodName][0].value;
            _returns[methodName].RemoveAt(0);

            return retval;
        }
    }
}
