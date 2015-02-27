namespace RdClient.Shared.CxWrappers.Errors
{
    public class XPlatError : IRdpError
    {
        public enum XResult32
        {
            Succeeded,
            OutOfMemory,
            NoInterface,
            NoObject,
            BadParameter,
            ObjectNotInitialized,
            OutOfRange,
            Rundown,
            Unexpected,
            InBufTooSmall,
            WorkspaceExists,
            ObjectAlreadyInitialized,
            NotImplemented,
            StreamClosed,
            EndOfStream,
            InvalidString,
            ObjectAlreadyExists,
            ProtocolError,
            Cancelled,
            InvalidURL,
            InvalidWorkspaceFeed,
            NoValidIconExists,
            UnsupportedResourceType,
            UnsupportedURLScheme,
            ConnectionFailed,
            UnexpectedServerStatus,
            AuthenticationFailed,
            Timeout,
            FileWriteFailed,
            FileReadFailed,
            InvalidServerName,
            SecCertExpired,
            SecCertUnknown,
            SecCertWrongUsage,
            SecDelegationPolicy,
            SecDowngradeDetected,
            SecInternalError,
            SecLogonDenied,
            SecMutualAuthFailed,
            SecNoAuthenticationAuthority,
            SecNoCredentials,
            SecPolicyNtlmOnly,
            SecTimeSkew,
            SecUnsupportedFunction,
            SecWrongPrincipal,
            CertCnNoMatch,
            CertRevocationFailure,
            CryptRevoked,
            CryptRevocationOffline,
            BufferTooSmall,
            UnsupportedGraphicsMode,
            UnsupportedTapMessage,
            WouldBlock,
            SocketTimedOut,
            DnsLookupFailed,
            ConnectionReset,
            MoreProcessingRequired,
            HttpDenied,
            HttpProxyAuthReq,
            HttpStreamError,
            LoopbackFailed,
            BadProtocolPacket,
            BadIcon,
            HttpResendRequest,
            CertInvalidCA,
            CertInvalid,
            CertDateInvalid,
            CertRevoked,
            InvalidServerCert,
            SecChannelError,
            ServerUnavailable,
            NoMoreItems,
            InvalidOperation,
            ClientAuthCertNeeded,
            RedirectFailed,
            HttpHeaderNotFound,
            ClientCertPrivateKeyError,
            PasswordExpired,
            InvalidDiscoveryXml,
            InvalidConsentStatusUpdateResultXml,
            WorkspaceResourceNotFound,
            UserMismatch,
            AccessDenied,
            Failed,
            Unknown
        };

        private readonly XResult32 _xresult;
        public XResult32 XResult32Type { get { return _xresult; } }

        private readonly string _xresultCategory;
        public string XResultCategory { get { return _xresultCategory; } }

        private readonly object _context;
        public object Context { get { return _context; } }

        public XPlatError(XResult32 xresult, string xresultCategory, object context)
        {
            _xresult = xresult;
            _xresultCategory = xresultCategory;
            _context = Context;
        }

        public override string ToString()
        {
            return "XResult error: " + this.XResult32Type + " category: " + this.XResultCategory;
        }
    }
}
