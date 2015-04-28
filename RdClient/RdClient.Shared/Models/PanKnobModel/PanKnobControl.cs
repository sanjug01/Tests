using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobControl : IPanKnobControl
    {
        private readonly Point _nullPoint = new Point(0, 0);
        private IViewport _viewport;
        private IPanKnob _panKnob;

        public PanKnobControl(IViewport viewport, IPanKnob panKnob)
        {
            _viewport = viewport;
        }

        void IPanKnobControl.Move(double dx, double dy)
        {
            Point current = _panKnob.Position;
            _panKnob.Position = new Point(current.X + dx, current.Y + dy);
        }

        void IPanKnobControl.Pan(double dx, double dy)
        {
            _viewport.PanAndZoom(_nullPoint, dx, dy, 1.0, 0.0);
        }

        void IPanKnobControl.Zoom(Point center, Point translation, double scale)
        {
            _viewport.PanAndZoom(center, translation.X, translation.Y, scale, 0.0);
        }
    }
}
