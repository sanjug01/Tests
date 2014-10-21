using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
namespace RdClient.CxWrappers.Utils
{
    public class RdpConnectionCreator
    {
        public static RdpConnection CreateInstance(CoreWindow spWindow, SwapChainPanel spPanel)
        {
            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnection.CreateInstance(spWindow, spPanel, out rdpConnectionCx);

            return new RdpConnection(rdpConnectionCx);
        }
    }
}
