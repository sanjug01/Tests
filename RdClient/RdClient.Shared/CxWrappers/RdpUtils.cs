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
            properties.SetBoolProperty("Administrative Session", desktop.IsUseAdminSession);
            properties.SetIntProperty("AudioMode", desktop.AudioMode);

            // left mouse is not a property
            properties.SetLeftHandedMouseMode(desktop.IsSwapMouseButtons);
        }

        public static void ApplyScreenSize(IRdpProperties properties, IPhysicalScreenSize screenSize)
        {
            ScreenSize size = screenSize.GetScreenSize();
            properties.SetIntProperty("PhysicalDesktopWidth", size.Width);
            properties.SetIntProperty("PhysicalDesktopHeight", size.Height);
        }
    }
}
