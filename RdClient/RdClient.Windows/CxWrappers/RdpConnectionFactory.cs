using RdClient.Shared.CxWrappers;
using RdClient.Shared.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
namespace RdClient.CxWrappers.Utils
{
    public class RdpConnectionFactory
    {
        public static IRdpConnection CreateInstance(CoreWindow spWindow, SwapChainPanel swapChainPanel, SessionViewModel sessionViewModel)
        {
            int xRes;

            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnectionStore rdpConnectionStore;

            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStore);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            rdpConnectionStore.SetSwapChainPanel(swapChainPanel);

            xRes = rdpConnectionStore.CreateConnectionWithSettings("", out rdpConnectionCx);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a desktop connection with the given settings.");

            RdpEventHandlers eventHandlers = new RdpEventHandlers(sessionViewModel);

            return new RdpConnection(rdpConnectionCx, eventHandlers);
        }
    }
}
