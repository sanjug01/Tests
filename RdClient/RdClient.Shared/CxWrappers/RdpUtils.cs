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

            // extra settings - apply only if not the default values
            if (desktop.IsUseAdminSession)
            {
                properties.SetBoolProperty("Administrative Session", desktop.IsUseAdminSession);
            }

            if (0 != desktop.AudioMode)
            {
                properties.SetIntProperty("AudioMode", desktop.AudioMode);
            }

            if (desktop.IsSwapMouseButtons)
            {
                // left mouse is not a property
                properties.SetLeftHandedMouseMode(desktop.IsSwapMouseButtons);
            }
        }

        public static void ApplyScreenSize(IRdpProperties properties, IPhysicalScreenSize screenSize)
        {
            ScreenSize size = screenSize.GetScreenSize();
            properties.SetIntProperty("PhysicalDesktopWidth", size.Width);
            properties.SetIntProperty("PhysicalDesktopHeight", size.Height);
        }
    }
}
