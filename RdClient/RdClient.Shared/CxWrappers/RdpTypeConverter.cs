using RdClient.Shared.CxWrappers.Errors;
using System;

namespace RdClient.Shared.CxWrappers
{
    class RdpTypeConverter
    {
        public static RdClientCx.TouchEventType ConvertToCx(TouchEventType type)
        {
            switch(type)
            {
                case(TouchEventType.Down):
                    return RdClientCx.TouchEventType.TouchDown;
                case(TouchEventType.Up):
                    return RdClientCx.TouchEventType.TouchUp;
                case(TouchEventType.Update):
                    return RdClientCx.TouchEventType.TouchUpdate;
                case(TouchEventType.Unknown):
                    return RdClientCx.TouchEventType.TouchUp;
                default:
                    return RdClientCx.TouchEventType.TouchUp;
            }
        }

        public static RdClientCx.MouseEventType ConvertToCx(MouseEventType type)
        {
            switch(type)
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
            if (Enum.IsDefined(typeof(XPlatError.XResult32), error))
            {
                xresult = (XPlatError.XResult32) error;
            }
            else
            {
                xresult = XPlatError.XResult32.Unknown;
            }
            return xresult;
        }

        public static RadcError.RadcStatus ConvertFromCx(RdClientCx.RadcErrorCode radcErrorCode)
        {
            RadcError.RadcStatus radcStatus;

            switch (radcErrorCode)
            {
                case (RdClientCx.RadcErrorCode.ErrorCompleteFailure):
                    radcStatus = RadcError.RadcStatus.CompleteFailure;
                    break;
                case (RdClientCx.RadcErrorCode.ErrorNetworkFailure):
                    radcStatus = RadcError.RadcStatus.NetworkFailure;
                    break;
                case (RdClientCx.RadcErrorCode.ErrorPartialFailure):
                    radcStatus = RadcError.RadcStatus.PartialFailure;
                    break;
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
