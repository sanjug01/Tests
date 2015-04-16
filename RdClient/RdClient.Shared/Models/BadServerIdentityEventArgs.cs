namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class BadServerIdentityEventArgs : EventArgs
    {
        private readonly RdpDisconnectReason _reason;
        private IServerIdentityValidation _validation;

        public RdpDisconnectReason DisconnectReason
        {
            get { return _reason; }
        }

        public IServerIdentityValidation ObtainValidation()
        {
            Contract.Ensures(null != Contract.Result<ICertificateValidation>());
            Contract.Ensures(null == _validation);

            IServerIdentityValidation v = _validation;

            if (null == v)
                throw new InvalidOperationException("Already obtained server identity validation");
            _validation = null;

            return v;
        }

        public bool ValidationObtained { get { return null == _validation; } }

        public BadServerIdentityEventArgs(RdpDisconnectReason reason, IServerIdentityValidation validation)
        {
            Contract.Assert(null != reason);
            Contract.Assert(null != validation);
            Contract.Ensures(null != _reason);
            Contract.Ensures(null != _validation);

            _reason = reason;
            _validation = validation;
        }
    }
}
