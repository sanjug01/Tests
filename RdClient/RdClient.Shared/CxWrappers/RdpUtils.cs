using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers.Utils
{
    public class RdpPropertyApplier
    {
        public static void ApplyDesktop(
            IRdpProperties properties, 
            Desktop desktop)
        {
            properties.SetStringProperty("Full Address", desktop.HostName);
        }

        public static void ApplyScreenSize(IRdpProperties properties, IPhysicalScreenSize screenSize)
        {
            ScreenSize size = screenSize.GetScreenSize();
            properties.SetIntProperty("PhysicalDesktopWidth", size.Width);
            properties.SetIntProperty("PhysicalDesktopHeight", size.Height);
        }
    }

    class RdpTypeConverter
    {
        public static RdClientCx.RdpDisconnectCode ConvertToCxRdpDisconnectCode(RdpDisconnectCode disconnectCode)
        {
            return (RdClientCx.RdpDisconnectCode)disconnectCode;
        }
        public static RdpDisconnectCode ConvertFromCxRdpDisconnectCode(RdClientCx.RdpDisconnectCode disconnectCode)
        {
            return (RdpDisconnectCode)disconnectCode;
        }

        public static RdClientCx.RdpDisconnectReason ConvertToCxRdpDisconnectReason(RdpDisconnectReason disconnectReason)
        {
            RdClientCx.RdpDisconnectReason cxDisconnectReason = new RdClientCx.RdpDisconnectReason();

            cxDisconnectReason.code = RdpTypeConverter.ConvertToCxRdpDisconnectCode(disconnectReason.Code);
            cxDisconnectReason.uLegacyCode = disconnectReason.ULegacyCode;
            cxDisconnectReason.uLegacyExtendedCode = disconnectReason.ULegacyExtendedCode;

            return cxDisconnectReason;
        }

        public static RdpDisconnectReason ConvertFromCxRdpDisconnectReason(RdClientCx.RdpDisconnectReason cxDisconnectReason)
        {
            RdpDisconnectReason disconnectReason = new RdpDisconnectReason(
                RdpTypeConverter.ConvertFromCxRdpDisconnectCode(cxDisconnectReason.code),
                cxDisconnectReason.uLegacyCode,
                cxDisconnectReason.uLegacyExtendedCode);
            
            return disconnectReason;
        }
    }
}
