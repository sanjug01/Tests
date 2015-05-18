using System;
using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobControl : IPanKnobControl
    {
        private readonly Point _nullPoint = new Point(0, 0);
        private IViewport _viewport;
        public IViewport Viewport { set { _viewport = value; } }
        private IPanKnob _panKnob;

        private double tDx = 1;
        private double tDy = 1;

        public PanKnobControl(IPanKnob panKnob)
        {
            _panKnob = panKnob;
        }

        void IPanKnobControl.Complete()
        {
            tDx = 1;
            tDy = 1;
        }

        void IPanKnobControl.Move(double dx, double dy)
        {
            Point current = _panKnob.Position;

            if (current.X + dx < -(_viewport.Size.Width / 2) + (_panKnob.Size.Width / 2))
            {            
                current.X = -(_viewport.Size.Width / 2) + (_panKnob.Size.Width / 2);
                tDx = tDx * -1;
            }
            else if(current.X + _panKnob.Size.Width / 2 + dx > _viewport.Size.Width / 2)
            {
                current.X = _viewport.Size.Width / 2 - (_panKnob.Size.Width / 2);
                tDx = tDx * -1;
            }

            if (current.Y + dy < -(_viewport.Size.Height / 2) + (_panKnob.Size.Height / 2))
            {
                current.Y = -(_viewport.Size.Height / 2) + (_panKnob.Size.Height / 2);
                tDy = tDy * -1;
            }
            else if (current.Y + _panKnob.Size.Height / 2 + dy > _viewport.Size.Height / 2)
            {
                current.Y = _viewport.Size.Height / 2 - (_panKnob.Size.Height / 2);
                tDy = tDy * -1;
            }

            _panKnob.Position = new Point(current.X + dx * tDx, current.Y + dy * tDy);
        }

        void IPanKnobControl.Pan(double dx, double dy)
        {
            if(_viewport != null)
            {
                _viewport.PanAndZoom(_nullPoint, dx, dy, 1.0);
            }
        }

        void IPanKnobControl.Zoom(Point center, Point translation, double scale)
        {
            if(_viewport != null)
            {
                _viewport.PanAndZoom(center, translation.X, translation.Y, scale);
            }
        }
    }
}
