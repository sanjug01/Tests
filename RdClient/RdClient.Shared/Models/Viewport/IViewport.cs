namespace RdClient.Shared.Models.Viewport
{
    using System;
    using System.ComponentModel;
    using Windows.Foundation;

    public interface IViewport : INotifyPropertyChanged
    {
        IViewportPanel SessionPanel { get; }

        event EventHandler Changed;

        /// <summary>
        /// Absolute size of the viewport. When the size changes, INotifyPropertyChanged.PropertyChanged event is emitted.
        /// </summary>
        /// <remarks>All updates of the viewport happen on the UI thread.</remarks>
        Size Size { get; }

        /// <summary>
        /// Offset of the rendering panel relative to the viewport. The coordinate system is based on the upper left
        /// corner with the Y-coordinate growing downward.
        /// </summary>
        /// <remarks>The viewport updates the value when all transition animations have finished.</remarks>
        Point Offset { get; }

        /// <summary>
        /// Current zoom factor. The value is always greater than or equal to 1.0.
        /// </summary>
        /// <remarks>The viewport updates the value when all transition animations have finished.</remarks>
        double ZoomFactor { get; }

        /// <summary>
        /// Set the viewport zoom factor and offset.
        /// </summary>
        /// <param name="zoomFactor">Desired zoom factor.</param>
        /// <param name="offset">Desired offset.</param>
        /// <remarks>The viewport may adjust the desired zoom factor and offset values based on its internal logic.
        /// The adjusted values will be reported through the Offset and ZoomFactor properties.</remarks>
        void SetZoom(double zoomFactor, Point anchorPoint);
        void SetPan(double x, double y);

        /// <summary>
        /// Pan the view port and zoom the rendering panel.
        /// </summary>
        /// <param name="anchrorPoint">Anchor point of the pan/zoom operation - the point in the rendering panel user thinks she drags.</param>
        /// <param name="dx">Desired absolute horizontal shift of the anchor pooint in the viewport.</param>
        /// <param name="dy">Desired absolute vertical shift of the anchor pooint in the viewport.</param>
        /// <param name="scaleFactor">Multiplier for the current zoom factor; values less than 1 reduce the scale, values greater than 1
        /// increase the scale.</param>
        /// <remarks>
        /// <para>Zooming is performed around the anchor point, so the actual absolute shift of the viewport
        /// may differ from the desired one passed in the shift parameter.</para>
        /// <para>The method moves the anchor point in the viewport. When the anchor point goes to the right, the rendering panel
        /// actually scrolls to the left.</para>
        /// </remarks>
        void PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor);

        void Reset();
    }
}
