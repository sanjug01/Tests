namespace RdClient.Models
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// RDP connection source injected in the session factory navigation extension.
    /// Single instance of the class is created in XAML of the main page (MainPage.xaml)
    /// and upon the first request, the single object remembers the passed rendering panel
    /// and checks if it is passed for all subsequent calls of CreateSession.
    /// </summary>
    sealed class SingleSwapChainPanelRdpConnectionSource : IRdpConnectionSource
    {
        IRdpConnection IRdpConnectionSource.CreateConnection(RemoteConnectionModel model, IRenderingPanel renderingPanel)
        {
            Contract.Assert(null != model);
            Contract.Assert(null != renderingPanel);
            Contract.Assert(renderingPanel is SwapChainPanel);
            Contract.Ensures(null != Contract.Result<IRdpConnection>());

            IRdpConnectionFactory factory = new RdpConnectionFactory() { SwapChainPanel = (SwapChainPanel)renderingPanel };

            return model.CreateConnection(factory, renderingPanel);
        }
    }
}
