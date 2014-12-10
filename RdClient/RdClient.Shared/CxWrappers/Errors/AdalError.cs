namespace RdClient.Shared.CxWrappers.Errors
{
    public class AdalError : IRdpError
    {
        public enum AdalStatus
        {
            Failed,
            Succeeded,
            Canceled,
            UserMismatch,
            Denied,
            Unknown
        }

        private readonly AdalStatus _adalStatus;
        public AdalStatus AdalStatusType { get { return _adalStatus; } }
        public AdalError(AdalStatus adalStatus)
        {
            _adalStatus = adalStatus;
        }

        public string Category
        {
            get { return "AdalError"; }
        }
    }
}
