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
                    status = "Failed";
                    break;
                case (AdalStatus.Succeeded):
                    status = "Succeeded";
                    break;
                case (AdalStatus.Canceled):
                    status = "Canceled";
                    break;
                case (AdalStatus.UserMismatch):
                    status = "UserMismatch";
                    break;
                case (AdalStatus.Denied):
                    status = "Denied";
                    break;
                case (AdalStatus.Unknown):
                    status = "Denied";
                    break;
                default:
                    status = "undefined error?!";
                    break;
            }

            return "AdalError: " + status;
        }

        public string Category
        {
            get { return "AdalError"; }
        }
    }
}
