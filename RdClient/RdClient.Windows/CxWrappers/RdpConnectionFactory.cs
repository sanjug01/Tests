namespace RdClient.CxWrappers.Utils
{
    using RdClient.Shared.CxWrappers;
    using Windows.UI.Xaml.Controls;

    public sealed class RdpConnectionFactory : IRdpConnectionFactory
    {
        public SwapChainPanel SwapChainPanel { private get; set; }

        IRdpConnection IRdpConnectionFactory.CreateDesktop()
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


        IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
        {
            throw new System.NotImplementedException();
        }
    }
}
