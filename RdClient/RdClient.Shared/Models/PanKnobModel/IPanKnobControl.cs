using RdClient.Shared.Models.Viewport;
using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnobControl
    {
        IViewport Viewport { set; }

        void Move(double dx, double dy);
        void Pan(double dx, double dy);
        void Zoom(Point center, Point translation, double scale);
        void Complete();
    }
}
