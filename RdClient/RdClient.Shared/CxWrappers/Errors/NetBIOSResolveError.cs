namespace RdClient.Shared.CxWrappers.Errors
{
    public class NetBIOSResolveError : IRdpError
    {
        // I was not able to find any information on how to interpret this error code DT
        private readonly int _error;
        public int Error { get { return _error; } }
        public NetBIOSResolveError(int error)
        {
            _error = error;
        }

        public string ToString()
        {
            return "NetBIOSResolve Error: " + this.Error;
        }

        public string Category
        {
            get { return "NetBIOSResolveError"; }
        }
    }
}
