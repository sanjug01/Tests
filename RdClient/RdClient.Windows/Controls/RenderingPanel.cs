namespace RdClient.Controls
{
    using RdClient.Shared.Models;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Wrapper of SwapChainPanel that adds the IRenderingPanel interface.
    /// </summary>
    public sealed class RenderingPanel : SwapChainPanel, IRenderingPanel
    {
        public RenderingPanel()
        {
            //
            // Explicitly make the default constructor public.
            //
        }
    }
}
