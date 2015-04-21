namespace RdClient.Shared.Models
{
    using RdClient.Shared.Input;
    using RdClient.Shared.Input.Pointer;
    using System;
    using Windows.Foundation;

    /// <summary>
    /// Dummy interface that hides a rendering panel (SwapChainPanel) from the session infrastructure code.
    /// </summary>
    public interface IRenderingPanel
    {
        event EventHandler Ready;

        /// <summary>
        /// The event always emitted on a worker thread that captures input on the rendering panel.
        /// </summary>
        /// <remarks>The rendering panel starts capturing input when the first event handler is registered,
        /// and stops the capture when the last handler has been removed.</remarks>
        event EventHandler<IPointerEventBase> PointerChanged;

        /// <summary>
        /// Object that manages the viewport in which a part of the rendering panel is shown.
        /// </summary>
        /// <remarks>Technically, the viewport is the grid that hosts the rendering panel (swap chain panel);
        /// the rendering panel may be scaled up so the viewport will become smaller than the panel.</remarks>
        IViewport Viewport { get; }

        void ChangeMouseCursorShape(MouseCursorShape shape);
        void MoveMouseCursor(Point point);
    }
}
