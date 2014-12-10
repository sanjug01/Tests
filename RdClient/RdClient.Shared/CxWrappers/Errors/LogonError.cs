namespace RdClient.Shared.CxWrappers.Errors
{
    public class LogonError : IRdpError
    {
        // I was not able to find any information on how to interpret this error code DT
        private readonly int _error;
        public int Error { get { return _error; } }
        public LogonError(int error)
        {
            _error = error;
        }

        public string Category
        {
            get { return "LogonError"; }
        }
    }
}
