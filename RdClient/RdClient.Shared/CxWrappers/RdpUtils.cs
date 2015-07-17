using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using Windows.Foundation;

namespace RdClient.Shared.CxWrappers.Utils
{
    public class RdpPropertyApplier
    {
        public static void ApplyDesktop(
            IRdpProperties properties, 
            DesktopModel desktop)
        {
            properties.SetStringProperty("Full Address", desktop.HostName);
            properties.SetBoolProperty("Administrative Session", desktop.IsAdminSession);
            properties.SetIntProperty("AudioMode", (int) desktop.AudioMode);

            properties.SetIntProperty("DesktopScaleFactor", (int)150);
            properties.SetIntProperty("DeviceScaleFactor", (int)100);

            properties.SetBoolProperty("High Resolution Mouse", true);
        }

        public static void ApplyScreenSize(IRdpProperties properties, IWindowSize windowSize)
        {
            Size size = windowSize.Size;
            properties.SetIntProperty("PhysicalDesktopWidth", (int) size.Width);
            properties.SetIntProperty("PhysicalDesktopHeight", (int) size.Height);
        }
    }
}
