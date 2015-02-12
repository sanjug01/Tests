using System.Reflection;

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
}
