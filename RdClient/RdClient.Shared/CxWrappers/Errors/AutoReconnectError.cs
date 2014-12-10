namespace RdClient.Shared.CxWrappers.Errors
{
    public class AutoReconnectError : IRdpError
    {
        // I was not able to find any information on how to interpret this error code DT
        private readonly int _error;
        public int Error { get { return _error; } }
        public AutoReconnectError(int error)
        {
            _error = error;
        }

        public override string ToString()
        {
            return "AutoReconnectError: " + this.Error;
        }

        public string Category
        {
            get { return "AutoReconnectError"; }
        }
    }
}
