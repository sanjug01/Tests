using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Helpers
{
    public class DisconnectString
    {
        private readonly IStringTable _localizedStrings;

        private Dictionary<RdpDisconnectCode, string> codeMap = new Dictionary<RdpDisconnectCode,string>();

        public DisconnectString(IStringTable localizedStrings)
        {
            _localizedStrings = localizedStrings;

            codeMap[RdpDisconnectCode.AccountDisabled] = "Disconnect_AccountDisabled_String";
            codeMap[RdpDisconnectCode.AccountExpired] = "Disconnect_AccountExpired_String";
            codeMap[RdpDisconnectCode.AccountLockedOut] = "Disconnect_AccountLockedOut_String";
            codeMap[RdpDisconnectCode.AccountRestriction] = "Disconnect_AccountRestriction_String";
            codeMap[RdpDisconnectCode.ArcFailed] = "Disconnect_ArcFailed_String";
            codeMap[RdpDisconnectCode.CantUpgradeLicense] = "Disconnect_CantUpgradeLicense_String";
            codeMap[RdpDisconnectCode.CertExpired] = "Disconnect_CertExpired_String";
            codeMap[RdpDisconnectCode.CertMismatch] = "Disconnect_CertMismatch_String";
            //codeMap[RdpDisconnectCode.CertValidationFailed] = "";
            codeMap[RdpDisconnectCode.ConnectionBroken] = "Disconnect_ConnectionBroken_String";
            codeMap[RdpDisconnectCode.ConnectionTimeout] = "Disconnect_ConnectionTimeout_String";
            //codeMap[RdpDisconnectCode.CredSSPUnsupported] = "";
            codeMap[RdpDisconnectCode.DecompressionFailed] = "Disconnect_DecompressionFailed_String";
            codeMap[RdpDisconnectCode.EncryptionFailed] = "Disconnect_EncryptionFailed_String";
            //codeMap[RdpDisconnectCode.FreshCredsRequired] = "";
            codeMap[RdpDisconnectCode.GenericByServer] = "Disconnect_GenericByServer_String";
            codeMap[RdpDisconnectCode.GenericLicenseError] = "Disconnect_GenericLicenseError_String";
            codeMap[RdpDisconnectCode.GenericNetworkError] = "Disconnect_GenericNetworkError_String";
            codeMap[RdpDisconnectCode.GenericProtocolError] = "Disconnect_GenericProtocolError_String";
            codeMap[RdpDisconnectCode.GenericSecurityError] = "Disconnect_GenericSecurityError_String";
            codeMap[RdpDisconnectCode.HybridRequired] = "Disconnect_HybridRequired_String";
            codeMap[RdpDisconnectCode.IdleTimeout] = "Disconnect_IdleTimeout_String";
            codeMap[RdpDisconnectCode.InitFipsFailed] = "Disconnect_InitFipsFailed_String";
            codeMap[RdpDisconnectCode.InvalidLicense] = "Disconnect_InvalidLicense_String";
            codeMap[RdpDisconnectCode.InvalidLogonHours] = "Disconnect_InvalidLogonHours_String";
            codeMap[RdpDisconnectCode.InvalidWorkStation] = "Disconnect_InvalidWorkStation_String";
            codeMap[RdpDisconnectCode.KrbUser2UserRequired] = "Disconnect_KrbUser2UserRequired_String";
            codeMap[RdpDisconnectCode.LogonTimeout] = "Disconnect_LogonTimeout_String";
            codeMap[RdpDisconnectCode.LogonTypeNotGranted] = "Disconnect_LogonTypeNotGranted_String";
            codeMap[RdpDisconnectCode.LoopbackUnsupported] = "Disconnect_LoopbackUnsupported_String";
            codeMap[RdpDisconnectCode.NoLicenseAvailable] = "Disconnect_NoLicenseAvailable_String";
            codeMap[RdpDisconnectCode.NoLicenseServer] = "Disconnect_NoLicenseServer_String";
            codeMap[RdpDisconnectCode.NoRemoteConnectionLicense] = "Disconnect_NoRemoteConnectionLicense_String";
            codeMap[RdpDisconnectCode.NoSuchUser] = "Disconnect_NoSuchUser_String";
            codeMap[RdpDisconnectCode.PasswordExpired] = "Disconnect_PasswordExpired_String";
            codeMap[RdpDisconnectCode.PasswordMustChange] = "Disconnect_PasswordMustChange_String";
            //codeMap[RdpDisconnectCode.PreAuthLogonFailed] = "";
            codeMap[RdpDisconnectCode.RemotingDisabled] = "Disconnect_RemotingDisabled_String";
            codeMap[RdpDisconnectCode.ReplacedByOtherConnection] = "Disconnect_ReplacedByOtherConnection_String";
            codeMap[RdpDisconnectCode.ServerDeniedConnection] = "Disconnect_ServerDeniedConnection_String";
            codeMap[RdpDisconnectCode.ServerInsufficientPrivileges] = "Disconnect_ServerInsufficientPrivileges_String";
            codeMap[RdpDisconnectCode.ServerNameLookupFailed] = "Disconnect_ServerNameLookupFailed_String";
            codeMap[RdpDisconnectCode.ServerOutOfMemory] = "Disconnect_ServerOutOfMemory_String";
            codeMap[RdpDisconnectCode.ServerTool] = "Disconnect_ServerTool_String";
            codeMap[RdpDisconnectCode.SSLHandshakeFailed] = "Disconnect_SSLHandshakeFailed_String";
            codeMap[RdpDisconnectCode.TimeSkew] = "Disconnect_TimeSkew_String";
            codeMap[RdpDisconnectCode.UnknownError] = "Disconnect_UnknownError_String";
            //codeMap[RdpDisconnectCode.UserInitiated] = "";
            codeMap[RdpDisconnectCode.VersionMismatch] = "Disconnect_VersionMismatch_String";
        }

        public string GetDisconnectString(RdpDisconnectReason reason)
        {
            string localizedKey;
            string localizedValue;

            if(codeMap.ContainsKey(reason.Code))
            {
                localizedKey = codeMap[reason.Code];
            }
            else
            {
                localizedKey = "Disconnect_UnknownError_String";
            }

            localizedValue = _localizedStrings.GetLocalizedString(localizedKey);

            localizedValue += "\n\n";

            if (0 != reason.ULegacyExtendedCode)
            {
                localizedValue += String.Format(
                    _localizedStrings.GetLocalizedString("Disconnect_ExtendedErrorCode_String"),
                    reason.ULegacyExtendedCode.ToString("X") // format number as hexadecimal
                    );
            }
            else
            {
                localizedValue += String.Format(
                    _localizedStrings.GetLocalizedString("Disconnect_ErrorCode_String"),
                    reason.ULegacyCode.ToString("X")  // format number as hexadecimal
                    );
            }

            return localizedValue;
        }
    }
}
