namespace RdClient.Shared.CxWrappers.Errors
{
    public class RadcError : IRdpError
    {
        public enum RadcStatus
        {
            Success,
            PartialFailure,
            CompleteFailure,
            CompleteFailureOnFirstSignIn,
            NetworkFailure,
            Unknown
        }

        private readonly RadcStatus _radcStatus;
        public RadcStatus RadcErrorType { get { return _radcStatus; } }
        public RadcError(RadcStatus radcStatus)
        {
            _radcStatus = radcStatus;
        }

        public string Category
        {
            get { return "RadcError"; }
        }
    }
}
