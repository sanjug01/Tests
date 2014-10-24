using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers
{
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

    public struct RdpDisconnectReason
    {
        public RdpDisconnectCode code;
        public uint uLegacyCode;
        public uint uLegacyExtendedCode;
    }

    public interface IRdpConnection
    {
        void Connect(Credentials credentials, bool fUsingSavedCreds);
        void Disconnect();
        void Suspend();
        void Resume();
        int HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer);
    }
}
