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

        public override string ToString()
        {
            string status = "";

            switch(this.AdalStatusType)
            {
                case (AdalStatus.Failed):
                case (AdalStatus.Succeeded):
                case (AdalStatus.Canceled):
                case (AdalStatus.UserMismatch):
                case (AdalStatus.Denied):
                case (AdalStatus.Unknown):
                    status = this.AdalStatusType.ToString();
                    break;
                default:
                    status = "undefined error?!";
                    break;
            }

            return "AdalError: " + status;
        }
    }
}
