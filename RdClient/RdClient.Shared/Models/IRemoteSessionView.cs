namespace RdClient.Shared.Models
{
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models.Viewport;
    using System;
    using System.ComponentModel;
    using Windows.Foundation;

    /// <summary>
    /// Interface of a single view that renders contents of a single RDP protocol monitor.
    /// Main purpose of the view is to provide a rendering panel to the remote session object.
    /// In the production application all methods of the interface must be called,
    /// and all events are emitted on the UI thread.
    /// </summary>
    public interface IRemoteSessionView : IViewportPanel, INotifyPropertyChanged
    {
        /// <summary>
        /// The event always emitted on a worker thread that captures input on the rendering panel.
        /// </summary>
        /// <remarks>The rendering panel starts capturing input when the first event handler is registered,
        /// and stops the capture when the last handler has been removed.</remarks>
        event EventHandler<IPointerEventBase> PointerChanged;

        /// <summary>
        /// User has closed the view. The event may be emitted by auxilliary session views opened to render
        /// additional monitors. The session view shown in the main application UI never emits the event.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Obtain a new regdering panel (SwapChainPanel in the production application).
        /// </summary>
        /// <returns>A new clean rendering panel that an RDP session may use to render a remote desktop.</returns>
        /// <remarks>Each rendering panel must be recycled when it is no longer needed.</remarks>
        IRenderingPanel ActivateNewRenderingPanel();

        /// <summary>
        /// Recycle a rendering panel returned earlier by <see cref="ActivateNewRenderingPanel">ActivateRenderingPanel</see>
        /// </summary>
        /// <param name="renderingPanel">Rendering panel to be recycled.</param>
        void RecycleRenderingPanel(IRenderingPanel renderingPanel);
    }
}
