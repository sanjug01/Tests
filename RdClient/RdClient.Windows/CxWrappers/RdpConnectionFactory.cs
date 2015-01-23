﻿using RdClient.Shared.CxWrappers;
using Windows.UI.Xaml.Controls;
namespace RdClient.CxWrappers.Utils
{
    public class RdpConnectionFactory : IRdpConnectionFactory
    {
        private const double MAX_ZOOM_FACTOR = 2.5;
        public SwapChainPanel SwapChainPanel { private get; set; }

        public IRdpConnection CreateInstance()
        {
            int xRes;

            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnectionStore rdpConnectionStoreCx;

            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStoreCx);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            rdpConnectionStoreCx.SetSwapChainPanel(SwapChainPanel);

            xRes = rdpConnectionStoreCx.CreateConnectionWithSettings("", out rdpConnectionCx);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a desktop connection with the given settings.");

            return new RdpConnection(rdpConnectionCx, rdpConnectionStoreCx, new RdpEventSource());
        }
    }
}
