using RdClient.Shared.CxWrappers.Errors;

namespace RdClient.Shared.CxWrappers
{
    class RdpTypeConverter
    {
        public static RdClientCx.MouseEventType ConvertToCx(MouseEventType cxType)
        {
            switch(cxType)
            {
                case (MouseEventType.LeftPress):
                    return RdClientCx.MouseEventType.LeftPress;
                case (MouseEventType.LeftRelease):
                    return RdClientCx.MouseEventType.LeftRelease;
                case (MouseEventType.RightPress):
                    return RdClientCx.MouseEventType.RightPress;
                case (MouseEventType.RightRelease):
                    return RdClientCx.MouseEventType.RightRelease;
                case (MouseEventType.Move):
                    return RdClientCx.MouseEventType.Move;
                case (MouseEventType.MouseWheel):
                    return RdClientCx.MouseEventType.MouseWheel;
                case (MouseEventType.MouseHWheel):
                    return RdClientCx.MouseEventType.MouseHWheel;
                default:
                    return RdClientCx.MouseEventType.LeftRelease;
            }
        }

        public static XPlatError.XResult32 ConvertFromCx(int error)
        {
            XPlatError.XResult32 xresult;
            switch (error)
            {
                case (0):
                    xresult = XPlatError.XResult32.Succeeded;
                    break;
                case (1):
                    xresult = XPlatError.XResult32.OutOfMemory;
                    break;
                case (2):
                    xresult = XPlatError.XResult32.NoInterface;
                    break;
                case (3):
                    xresult = XPlatError.XResult32.NoObject;
                    break;
                case (4):
                    xresult = XPlatError.XResult32.BadParameter;
                    break;
                case (5):
                    xresult = XPlatError.XResult32.ObjectNotInitialized;
                    break;
                case (6):
                    xresult = XPlatError.XResult32.OutOfRange;
                    break;
                case (7):
                    xresult = XPlatError.XResult32.Rundown;
                    break;
                case (8):
                    xresult = XPlatError.XResult32.Unexpected;
                    break;
                case (9):
                    xresult = XPlatError.XResult32.InBufTooSmall;
                    break;
                case (10):
                    xresult = XPlatError.XResult32.WorkspaceExists;
                    break;
                case (11):
                    xresult = XPlatError.XResult32.ObjectAlreadyInitialized;
                    break;
                case (12):
                    xresult = XPlatError.XResult32.NotImplemented;
                    break;
                case (13):
                    xresult = XPlatError.XResult32.StreamClosed;
                    break;
                case (14):
                    xresult = XPlatError.XResult32.EndOfStream;
                    break;
                case (15):
                    xresult = XPlatError.XResult32.InvalidString;
                    break;
                case (16):
                    xresult = XPlatError.XResult32.ObjectAlreadyExists;
                    break;
                case (17):
                    xresult = XPlatError.XResult32.ProtocolError;
                    break;
                case (18):
                    xresult = XPlatError.XResult32.Cancelled;
                    break;
                case (19):
                    xresult = XPlatError.XResult32.InvalidURL;
                    break;
                case (20):
                    xresult = XPlatError.XResult32.InvalidWorkspaceFeed;
                    break;
                case (21):
                    xresult = XPlatError.XResult32.NoValidIconExists;
                    break;
                case (22):
                    xresult = XPlatError.XResult32.UnsupportedResourceType;
                    break;
                case (23):
                    xresult = XPlatError.XResult32.UnsupportedURLScheme;
                    break;
                case (24):
                    xresult = XPlatError.XResult32.ConnectionFailed;
                    break;
                case (25):
                    xresult = XPlatError.XResult32.UnexpectedServerStatus;
                    break;
                case (26):
                    xresult = XPlatError.XResult32.AuthenticationFailed;
                    break;
                case (27):
                    xresult = XPlatError.XResult32.Timeout;
                    break;
                case (28):
                    xresult = XPlatError.XResult32.FileWriteFailed;
                    break;
                case (29):
                    xresult = XPlatError.XResult32.FileReadFailed;
                    break;
                case (30):
                    xresult = XPlatError.XResult32.InvalidServerName;
                    break;
                case (31):
                    xresult = XPlatError.XResult32.SecCertExpired;
                    break;
                case (32):
                    xresult = XPlatError.XResult32.SecCertUnknown;
                    break;
                case (33):
                    xresult = XPlatError.XResult32.SecCertWrongUsage;
                    break;
                case (34):
                    xresult = XPlatError.XResult32.SecDelegationPolicy;
                    break;
                case (35):
                    xresult = XPlatError.XResult32.SecDowngradeDetected;
                    break;
                case (36):
                    xresult = XPlatError.XResult32.SecInternalError;
                    break;
                case (37):
                    xresult = XPlatError.XResult32.SecLogonDenied;
                    break;
                case (38):
                    xresult = XPlatError.XResult32.SecMutualAuthFailed;
                    break;
                case (39):
                    xresult = XPlatError.XResult32.SecNoAuthenticationAuthority;
                    break;
                case (40):
                    xresult = XPlatError.XResult32.SecNoCredentials;
                    break;
                case (41):
                    xresult = XPlatError.XResult32.SecPolicyNtlmOnly;
                    break;
                case (42):
                    xresult = XPlatError.XResult32.SecTimeSkew;
                    break;
                case (43):
                    xresult = XPlatError.XResult32.SecUnsupportedFunction;
                    break;
                case (44):
                    xresult = XPlatError.XResult32.SecWrongPrincipal;
                    break;
                case (45):
                    xresult = XPlatError.XResult32.CertCnNoMatch;
                    break;
                case (46):
                    xresult = XPlatError.XResult32.CertRevocationFailure;
                    break;
                case (47):
                    xresult = XPlatError.XResult32.CryptRevoked;
                    break;
                case (48):
                    xresult = XPlatError.XResult32.CryptRevocationOffline;
                    break;
                case (49):
                    xresult = XPlatError.XResult32.BufferTooSmall;
                    break;
                case (50):
                    xresult = XPlatError.XResult32.UnsupportedGraphicsMode;
                    break;
                case (51):
                    xresult = XPlatError.XResult32.UnsupportedTapMessage;
                    break;
                case (52):
                    xresult = XPlatError.XResult32.WouldBlock;
                    break;
                case (53):
                    xresult = XPlatError.XResult32.SocketTimedOut;
                    break;
                case (54):
                    xresult = XPlatError.XResult32.DnsLookupFailed;
                    break;
                case (55):
                    xresult = XPlatError.XResult32.ConnectionReset;
                    break;
                case (56):
                    xresult = XPlatError.XResult32.MoreProcessingRequired;
                    break;
                case (57):
                    xresult = XPlatError.XResult32.HttpDenied;
                    break;
                case (58):
                    xresult = XPlatError.XResult32.HttpProxyAuthReq;
                    break;
                case (59):
                    xresult = XPlatError.XResult32.HttpStreamError;
                    break;
                case (60):
                    xresult = XPlatError.XResult32.LoopbackFailed;
                    break;
                case (61):
                    xresult = XPlatError.XResult32.BadProtocolPacket;
                    break;
                case (62):
                    xresult = XPlatError.XResult32.BadIcon;
                    break;
                case (63):
                    xresult = XPlatError.XResult32.HttpResendRequest;
                    break;
                case (64):
                    xresult = XPlatError.XResult32.CertInvalidCA;
                    break;
                case (65):
                    xresult = XPlatError.XResult32.CertInvalid;
                    break;
                case (66):
                    xresult = XPlatError.XResult32.CertDateInvalid;
                    break;
                case (67):
                    xresult = XPlatError.XResult32.CertRevoked;
                    break;
                case (68):
                    xresult = XPlatError.XResult32.InvalidServerCert;
                    break;
                case (69):
                    xresult = XPlatError.XResult32.SecChannelError;
                    break;
                case (70):
                    xresult = XPlatError.XResult32.ServerUnavailable;
                    break;
                case (71):
                    xresult = XPlatError.XResult32.NoMoreItems;
                    break;
                case (72):
                    xresult = XPlatError.XResult32.InvalidOperation;
                    break;
                case (73):
                    xresult = XPlatError.XResult32.ClientAuthCertNeeded;
                    break;
                case (74):
                    xresult = XPlatError.XResult32.RedirectFailed;
                    break;
                case (75):
                    xresult = XPlatError.XResult32.HttpHeaderNotFound;
                    break;
                case (76):
                    xresult = XPlatError.XResult32.ClientCertPrivateKeyError;
                    break;
                case (77):
                    xresult = XPlatError.XResult32.PasswordExpired;
                    break;
                case (78):
                    xresult = XPlatError.XResult32.InvalidDiscoveryXml;
                    break;
                case (79):
                    xresult = XPlatError.XResult32.InvalidConsentStatusUpdateResultXml;
                    break;
                case (80):
                    xresult = XPlatError.XResult32.WorkspaceResourceNotFound;
                    break;
                case (81):
                    xresult = XPlatError.XResult32.UserMismatch;
                    break;
                case (82):
                    xresult = XPlatError.XResult32.AccessDenied;
                    break;
                case (-1):
                    xresult = XPlatError.XResult32.Failed;
                    break;
                default:
                    xresult = XPlatError.XResult32.Unknown;
                    break;
            }
            
            return xresult;
        }

        public static RadcError.RadcStatus ConvertFromCx(RdClientCx.RadcErrorCode radcErrorCode)
        {
            RadcError.RadcStatus radcStatus;

            switch (radcErrorCode)
            {
                //case (RdClientCx.RadcErrorCode.ErrorCompleteFailure):
                //    radcStatus = RadcError.RadcStatus.CompleteFailure;
                //    break;
                //case (RdClientCx.RadcErrorCode.ErrorCompleteFailureOnFirstSignIn):
                //    radcStatus = RadcError.RadcStatus.CompleteFailureOnFirstSignIn;
                //    break;
                //case (RdClientCx.RadcErrorCode.ErrorNetworkFailure):
                //    radcStatus = RadcError.RadcStatus.NetworkFailure;
                //    break;
                //case (RdClientCx.RadcErrorCode.ErrorPartialFailure):
                //    radcStatus = RadcError.RadcStatus.PartialFailure;
                //    break;
                default:
                    radcStatus = RadcError.RadcStatus.Unknown;
                    break;
            }

            return radcStatus;
        }

        public static AdalError.AdalStatus ConvertFromCx(RdClientCx.GetAdalAccessTokenStatus getAdalAccessTokenStatus)
        {
            AdalError.AdalStatus adalStatus;

            switch (getAdalAccessTokenStatus)
            {
                case (RdClientCx.GetAdalAccessTokenStatus.Failed):
                    adalStatus = AdalError.AdalStatus.Failed;
                    break;
                case (RdClientCx.GetAdalAccessTokenStatus.Succeeded):
                    adalStatus = AdalError.AdalStatus.Succeeded;
                    break;
                case (RdClientCx.GetAdalAccessTokenStatus.Canceled):
                    adalStatus = AdalError.AdalStatus.Canceled;
                    break;
                case (RdClientCx.GetAdalAccessTokenStatus.Denied):
                    adalStatus = AdalError.AdalStatus.Denied;
                    break;
                case (RdClientCx.GetAdalAccessTokenStatus.UserMismatch):
                    adalStatus = AdalError.AdalStatus.UserMismatch;
                    break;
                default:
                    adalStatus = AdalError.AdalStatus.Unknown;
                    break;
            }

            return adalStatus;
        }

        public static RdClientCx.RdpDisconnectCode ConvertToCx(RdpDisconnectCode disconnectCode)
        {
            return (RdClientCx.RdpDisconnectCode)disconnectCode;
        }
        public static RdpDisconnectCode ConvertFromCx(RdClientCx.RdpDisconnectCode disconnectCode)
        {
            return (RdpDisconnectCode)disconnectCode;
        }

        public static RdClientCx.RdpDisconnectReason ConvertToCx(RdpDisconnectReason disconnectReason)
        {
            RdClientCx.RdpDisconnectReason cxDisconnectReason = new RdClientCx.RdpDisconnectReason();

            cxDisconnectReason.code = RdpTypeConverter.ConvertToCx(disconnectReason.Code);
            cxDisconnectReason.uLegacyCode = disconnectReason.ULegacyCode;
            cxDisconnectReason.uLegacyExtendedCode = disconnectReason.ULegacyExtendedCode;

            return cxDisconnectReason;
        }

        public static RdpDisconnectReason ConvertFromCx(RdClientCx.RdpDisconnectReason cxDisconnectReason)
        {
            RdpDisconnectReason disconnectReason = new RdpDisconnectReason(
                RdpTypeConverter.ConvertFromCx(cxDisconnectReason.code),
                cxDisconnectReason.uLegacyCode,
                cxDisconnectReason.uLegacyExtendedCode);

            return disconnectReason;
        }
    }
}
