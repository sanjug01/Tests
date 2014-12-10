namespace RdClient.Shared.CxWrappers.Errors
{
    // corresponds to $SDXROOT\termsrv\rdp\winrt\RdClientCx\RdpXPlatDisconnectReasons.h
    public enum RdpDisconnectCode
    {
        AccountDisabled = 0,
        AccountExpired = 1,
        AccountLockedOut = 2,
        AccountRestriction = 3,
        ArcFailed = 4,
        CantUpgradeLicense = 5,
        CertExpired = 6,
        CertMismatch = 7,
        CertValidationFailed = 8,
        ConnectionBroken = 9,
        ConnectionTimeout = 10,
        CredSSPUnsupported = 11,
        DecompressionFailed = 12,
        EncryptionFailed = 13,
        FreshCredsRequired = 14,
        GenericByServer = 15,
        GenericLicenseError = 16,
        GenericNetworkError = 17,
        GenericProtocolError = 18,
        GenericSecurityError = 19,
        HybridRequired = 20,
        IdleTimeout = 21,
        InitFipsFailed = 22,
        InvalidLicense = 23,
        InvalidLogonHours = 24,
        InvalidWorkStation = 25,
        KrbUser2UserRequired = 26,
        LogonTimeout = 27,
        LogonTypeNotGranted = 28,
        LoopbackUnsupported = 29,
        NoLicenseAvailable = 30,
        NoLicenseServer = 31,
        NoRemoteConnectionLicense = 32,
        NoSuchUser = 33,
        PasswordExpired = 34,
        PasswordMustChange = 35,
        PreAuthLogonFailed = 36,
        RemotingDisabled = 37,
        ReplacedByOtherConnection = 38,
        ServerDeniedConnection = 39,
        ServerInsufficientPrivileges = 40,
        ServerNameLookupFailed = 41,
        ServerOutOfMemory = 42,
        ServerTool = 43,
        SSLHandshakeFailed = 44,
        TimeSkew = 45,
        UnknownError = 46,
        UserInitiated = 47,
        VersionMismatch = 48,
    }

    public class RdpDisconnectReason : IRdpError
    {
        private readonly RdpDisconnectCode  _code;
        public RdpDisconnectCode Code { get { return _code; } }

        private readonly uint _uLegacyCode;
        public uint ULegacyCode { get { return _uLegacyCode; } }

        private readonly uint _uLegacyExtendedCode;
        public uint ULegacyExtendedCode { get { return _uLegacyExtendedCode; } }

        public RdpDisconnectReason(RdpDisconnectCode code, uint uLegacyCode, uint uLegacyExtendedCode)
        {
            _code = code;
            _uLegacyCode = uLegacyCode;
            _uLegacyExtendedCode = uLegacyExtendedCode;
        }

        public override string ToString()
        {
            return "Disconnect code: " + this.Code + ", legacy code: " + this.ULegacyCode + ", legacy extended code: " + this.ULegacyExtendedCode;
        }

        public string Category
        {
            get { return "RdpDisconnectReason"; }
        }
    }
}
