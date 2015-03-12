namespace RdClient.CxWrappers.Utils
{
    using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
    using Windows.UI.Xaml.Controls;


    public sealed class RdpConnectionFactory : IRdpConnectionFactory
    {
        public SwapChainPanel SwapChainPanel { private get; set; }

        public ConnectionInformation ConnectionInformation { private get; set; }

        IRdpConnection IRdpConnectionFactory.CreateDesktop()
        {
            int xRes;

            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnectionStore rdpConnectionStoreCx;

            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStoreCx);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            rdpConnectionStoreCx.SetSwapChainPanel(SwapChainPanel);

            RemoteApplicationModel app = this.ConnectionInformation.App;
            if (app != null)
            {
                xRes = rdpConnectionStoreCx.LaunchRemoteApp(app.RdpFile, out rdpConnectionCx);
            }
            else
            {
                xRes = rdpConnectionStoreCx.CreateConnectionWithSettings("", out rdpConnectionCx);
            }
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a desktop connection with the given settings.");
            return new RdpConnection(rdpConnectionCx, rdpConnectionStoreCx, new RdpEventSource());
        }


        IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
        {
            throw new System.NotImplementedException();
        }
    }
}
