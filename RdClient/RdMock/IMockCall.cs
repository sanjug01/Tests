using System;

namespace RdMock
{
    public class MockException : Exception
    {
        public MockException(string message) : base(message)
        { }
    }

    public interface IMockCall
    {
        string FunctionName { get; }
        object VerifyInvokedCall(MockInvokedCall actualCall);
    }
}
