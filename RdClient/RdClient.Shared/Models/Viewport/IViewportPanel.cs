using System.ComponentModel;

namespace RdClient.Shared.Models.Viewport
{
    public interface IViewportPanel : INotifyPropertyChanged
    {
        double Width { get; set; }
        double Height { get; set; }
        IViewportTransform Transform { get; }
    }
}