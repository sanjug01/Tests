using System;
using Windows.Foundation;
namespace RdClient.Shared.Models
{
    /// <summary>
    /// Dummy interface that hides a rendering panel (SwapChainPanel) from the session infrastructure code.
    /// </summary>
    public interface IRenderingPanel
    {
        event EventHandler Ready;
    }
}
