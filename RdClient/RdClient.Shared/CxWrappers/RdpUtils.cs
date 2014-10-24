using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.CxWrappers.Utils
{
    public class RdpPropertyApplier
    {
        public static void ApplyDesktop(
            IRdpProperties properties, 
            Desktop desktop)
        {
            properties.SetStringProperty("Full Address", desktop.hostName);
        }

        public static void ApplyScreenSize(IRdpProperties properties, IPhysicalScreenSize screenSize)
        {
            Tuple<int, int> size = screenSize.GetScreenSize();
            properties.SetIntProperty("PhysicalDesktopWidth", size.Item1);
            properties.SetIntProperty("PhysicalDesktopHeight", size.Item2);
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

            cxDisconnectReason.code = RdpTypeConverter.ConvertToCxRdpDisconnectCode(disconnectReason.code);
            cxDisconnectReason.uLegacyCode = disconnectReason.uLegacyCode;
            cxDisconnectReason.uLegacyExtendedCode = disconnectReason.uLegacyExtendedCode;

            return cxDisconnectReason;
        }

        public static RdpDisconnectReason ConvertFromCxRdpDisconnectReason(RdClientCx.RdpDisconnectReason cxDisconnectReason)
        {
            RdpDisconnectReason disconnectReason = new RdpDisconnectReason();

            disconnectReason.code = RdpTypeConverter.ConvertFromCxRdpDisconnectCode(cxDisconnectReason.code);
            disconnectReason.uLegacyCode = cxDisconnectReason.uLegacyCode;
            disconnectReason.uLegacyExtendedCode = cxDisconnectReason.uLegacyExtendedCode;

            return disconnectReason;
        }
    }
}
