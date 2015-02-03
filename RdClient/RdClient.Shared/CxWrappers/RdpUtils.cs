﻿using RdClient.Shared.Models;

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
        }

        public static void ApplyScreenSize(IRdpProperties properties, IPhysicalScreenSize screenSize)
        {
            ScreenSize size = screenSize.GetScreenSize();
            properties.SetIntProperty("PhysicalDesktopWidth", size.Width);
            properties.SetIntProperty("PhysicalDesktopHeight", size.Height);
        }
    }
}
