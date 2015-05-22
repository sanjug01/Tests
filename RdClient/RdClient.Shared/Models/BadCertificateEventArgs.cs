namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class BadCertificateEventArgs : EventArgs
    {
        private readonly RdpDisconnectReason _reason;
        private readonly string _hostName;
        private ICertificateValidation _validation;

        public RdpDisconnectReason DisconnectReason
        {
            get { return _reason; }
        }

        public string HostName
        {
            get { return _hostName; }
        }

        public ICertificateValidation ObtainValidation()
        {
            Contract.Ensures(null != Contract.Result<ICertificateValidation>());
            Contract.Ensures(null == _validation);

            ICertificateValidation v = _validation;

            if (null == v)
                throw new InvalidOperationException("Already obtained certificate validation");
            _validation = null;

            return v;
        }

        public bool ValidationObtained { get { return null == _validation; } }

        public BadCertificateEventArgs(RdpDisconnectReason reason, string hostName, ICertificateValidation validation)
        {
            Contract.Assert(null != reason);
            Contract.Assert(null != validation);
            Contract.Ensures(null != _reason);
            Contract.Ensures(null != _validation);

            _reason = reason;
            _validation = validation;
            _hostName = hostName;
        }
    }
}
