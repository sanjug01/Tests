namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models.Viewport;
    using System;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;



    /// <summary>
    /// Dummy interface that hides a rendering panel (SwapChainPanel) from the session infrastructure code.
    /// </summary>
    public interface IRenderingPanel
    {
        event EventHandler Ready;


        /// <summary>
        /// Object that manages the viewport in which a part of the rendering panel is shown.
        /// </summary>
        /// <remarks>Technically, the viewport is the grid that hosts the rendering panel (swap chain panel);
        /// the rendering panel may be scaled up so the viewport will become smaller than the panel.</remarks>
        IViewport Viewport { get; }
        IScaleFactor ScaleFactor { get; }

        void ChangeMouseCursorShape(ImageSource shape, Point hotspot);
        void MoveMouseCursor(Point point);
        void ChangeMouseVisibility(Visibility visibility);
        void ScaleMouseCursor(double scale);

    }
}
