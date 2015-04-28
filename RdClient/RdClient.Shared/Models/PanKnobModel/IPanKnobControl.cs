using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnobControl
    {
        void Move(double dx, double dy);
        void Pan(double dx, double dy);
        void Zoom(Point center, Point translation, double scale);
    }
}
