using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdMock
{
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
}
