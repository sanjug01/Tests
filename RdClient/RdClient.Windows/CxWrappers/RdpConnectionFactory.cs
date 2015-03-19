namespace RdClient.CxWrappers.Utils
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Models;
    using System;
    using Windows.UI.Xaml.Controls;


    public sealed class RdpConnectionFactory : IRdpConnectionFactory
    {
        public SwapChainPanel SwapChainPanel { private get; set; }

        public ConnectionInformation ConnectionInformation { private get; set; }

        IRdpConnection IRdpConnectionFactory.CreateDesktop(string rdpFile)
        {
            int xRes;

            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnectionStore rdpConnectionStoreCx;

            if (String.IsNullOrWhiteSpace(rdpFile))
            {
                rdpFile = "";
            }

            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStoreCx);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            rdpConnectionStoreCx.SetSwapChainPanel(SwapChainPanel);

            xRes = rdpConnectionStoreCx.CreateConnectionWithSettings(rdpFile, out rdpConnectionCx);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a desktop connection with the given settings.");
            return new RdpConnection(rdpConnectionCx, rdpConnectionStoreCx, new RdpEventSource());
        }


        IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
        {
            int xRes;

            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnectionStore rdpConnectionStoreCx;

            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStoreCx);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            rdpConnectionStoreCx.SetSwapChainPanel(SwapChainPanel);

            xRes = rdpConnectionStoreCx.LaunchRemoteApp(rdpFile, out rdpConnectionCx);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a remote app connection with the given settings.");
            return new RdpConnection(rdpConnectionCx, rdpConnectionStoreCx, new RdpEventSource());
        }
    }
}
