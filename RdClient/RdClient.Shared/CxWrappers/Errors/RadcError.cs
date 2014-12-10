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

        public override string ToString()
        {
            string status = "";

            switch (this.RadcErrorType)
            {
                case (RadcStatus.Success):
                    status = "Success";
                    break;
                case (RadcStatus.PartialFailure):
                    status = "PartialFailure";
                    break;
                case (RadcStatus.CompleteFailure):
                    status = "CompleteFailure";
                    break;
                case (RadcStatus.CompleteFailureOnFirstSignIn):
                    status = "CompleteFailureOnFirstSignIn";
                    break;
                case (RadcStatus.NetworkFailure):
                    status = "NetworkFailure";
                    break;
                case (RadcStatus.Unknown):
                    status = "Unknown";
                    break;
                default:
                    status = "undefined error?!";
                    break;
            }

            return "RadcError: " + status;
        }

        public string Category
        {
            get { return "RadcError"; }
        }
    }
}
