namespace RdClient.Shared.Models
{
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
        event EventHandler<PointerEventArgs> PointerChanged;

        void ChangeMouseCursorShape(MouseCursorShape shape);
        void MoveMouseCursor(Point point);
    }
}
