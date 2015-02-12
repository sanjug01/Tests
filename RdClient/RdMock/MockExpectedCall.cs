using System;
using System.Collections.Generic;
using System.Reflection;

namespace RdMock
{

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
                throw new MockException(string.Format("Mock call to {0}() failed. Expected {1} parameters but invoked with {2}.",
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

                if (Assignable.IsAssignable(actualType, expectedType) != actualType.IsAssignableFrom(expectedType))
                {

                }

                if(Assignable.IsAssignable(actualType, expectedType) == false)
                //if (actualType.IsAssignableFrom(expectedType) == false)
                {
                    throw new MockException(string.Format("Mock call to {0}() failed. Parameter {1} is of type {2} but expected type is {3}",
                                                        this.FunctionName, i, actualType.Name, expectedType.Name));
                }

                object expectedValue = expectedParameters[i];
                object actualValue = actualParameterValues[i];
                if (actualValue.Equals(expectedValue) == false)
                {
                    throw new MockException(string.Format("Mock call to {0}() failed. Wrong value for parameter {1}. Expected value = {2}, Actual value = {3}",
                                                        this.FunctionName, i, expectedValue, actualValue));
                }
            }

            return _returnValue;
        }
    }
}
