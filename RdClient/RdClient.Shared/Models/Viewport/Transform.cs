using Windows.Foundation;

namespace RdClient.Shared.Models.Viewport
{
    public class TransformX : IViewportTransform
    {
        private double _scaleX;
        public double ScaleX
        {
            get { return _scaleX; }
            set
            {
                _scaleX = value;
                _panel.Width *= _scaleX;
            }
        }

        private double _scaleY;
        public double ScaleY
        {
            get { return _scaleY; }
            set
            {
                _scaleY = value;
                _panel.Height *= _scaleY;
            }
        }

        private double _translateX;
        public double TranslateX
        {
            get { return _translateX; }
            set
            {
                _translateX = value;
            }
        }

        private double _translateY;
        public double TranslateY
        {
            get { return _translateY; }
            set
            {
                _translateY = value;
            }
        }

        public Point TransformPoint(Point point)
        {
            Point newPoint = new Point(point.X, point.Y);

            newPoint.X *= ScaleX;
            newPoint.Y *= ScaleY;
            newPoint.X += TranslateX;
            newPoint.Y += TranslateY;

            return newPoint;
        }

        public Point InverseTransformPoint(Point point)
        {
            Point newPoint = new Point(point.X, point.Y);

            newPoint.X *= 1.0 / ScaleX;
            newPoint.Y *= 1.0 / ScaleY;
            newPoint.X -= TranslateX;
            newPoint.Y -= TranslateY;

            return newPoint;
        }

        private IViewportPanel _panel;
        public TransformX(IViewportPanel panel)
        {
            _panel = panel;
            _scaleX = 1.0;
            _scaleY = 1.0;
        }
    }
}
