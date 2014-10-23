using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers.Utils
{
    public class RdpPropertyApplier
    {
        public static void ApplyRdpProperties(
            IRdpProperties properties, 
            Desktop desktop)
        {
            properties.SetStringProperty("Full Address", desktop.hostName);
        }
    }
}
