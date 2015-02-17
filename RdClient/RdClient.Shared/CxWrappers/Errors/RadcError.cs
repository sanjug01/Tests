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
                case (RadcStatus.PartialFailure):
                case (RadcStatus.CompleteFailure):
                case (RadcStatus.CompleteFailureOnFirstSignIn):
                case (RadcStatus.NetworkFailure):
                case (RadcStatus.Unknown):
                    status = this.RadcErrorType.ToString();
                    break;
                default:
                    status = "undefined error?!";
                    break;
            }

            return "RadcError: " + status;
        }
    }
}
