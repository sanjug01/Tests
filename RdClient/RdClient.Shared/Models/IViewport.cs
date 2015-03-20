namespace RdClient.Shared.Models
{
    using System.ComponentModel;
    using Windows.Foundation;

    public interface IViewport : INotifyPropertyChanged
    {
        /// <summary>
        /// Absolute size of the viewport. When the size changes, INotifyPropertyChanged.PropertyChanged event is emitted.
        /// </summary>
        /// <remarks>All updates of the viewport happen on the UI thread.</remarks>
        Size Size { get; }

        /// <summary>
        /// Pan the view port and zoom the rendering panel.
        /// </summary>
        /// <param name="anchrorPoint">Anchor point of the pan/zoom operation - the point in the rendering panel user thinks she drags.</param>
        /// <param name="shift">Desired absolute shift of the viewport.</param>
        /// <param name="scaleFactor">Multiplier for the current zoom factor; values less than 1 reduce the scale, values greater than 1
        /// increase the scale.</param>
        /// <remarks>Zooking is performed around the anchor point, so the actual absolute shift of the viewport
        /// may differ from the desired one passed in the shift parameter.</remarks>
        void PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor, double durationMilliseconds);
    }
}
