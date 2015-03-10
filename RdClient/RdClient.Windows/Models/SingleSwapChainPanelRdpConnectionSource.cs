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
        private IRdpConnectionFactory _factory;
        private IRenderingPanel _renderingPanel;

        IRdpConnection IRdpConnectionSource.CreateConnection(RemoteConnectionModel model, IRenderingPanel renderingPanel)
        {
            Contract.Assert(null != model);
            Contract.Assert(null != renderingPanel);
            Contract.Assert(renderingPanel is SwapChainPanel);
            Contract.Ensures(null != Contract.Result<IRdpConnection>());

            if(null == _factory)
            {
                //
                // Create the connection factory singleton and assign it the swap chain panel.
                //
                _renderingPanel = renderingPanel;
                _factory = new RdpConnectionFactory() { SwapChainPanel = (SwapChainPanel)renderingPanel };
            }
            else
            {
                //
                // Verify that the same swap chain panel is reused for all RDP connections.
                //
                if (!object.ReferenceEquals(_renderingPanel, renderingPanel))
                    throw new ArgumentException("Unexpected rendering panel", "renderingPanel");
            }
            //
            // Create and set up the RDP connection.
            //
            IRdpConnection rdpc = _factory.CreateInstance();
            IRdpProperties prop = rdpc as IRdpProperties;

            Contract.Assert(null != prop);

            model.SetUpConnection(prop);

            return rdpc;
        }
    }
}
