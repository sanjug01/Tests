using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Models.Viewport
{
    public class ViewportTransformWrapper : IViewportTransform
    {
        private CompositeTransform _transform;
        public ViewportTransformWrapper(CompositeTransform transform)
        {
            _transform = transform;
        }

        public double ScaleX
        {
            get
            {
                return _transform.ScaleX;
            }

            set
            {
                _transform.ScaleX = value;
            }
        }

        public double ScaleY
        {
            get
            {
                return _transform.ScaleY;
            }

            set
            {
                _transform.ScaleY = value;
            }
        }

        public double TranslateX
        {
            get
            {
                return _transform.TranslateX;
            }

            set
            {
                _transform.TranslateX = value;
            }
        }

        public double TranslateY
        {
            get
            {
                return _transform.TranslateY;
            }

            set
            {
                _transform.TranslateY = value;
            }
        }

        public Point TransformPoint(Point point)
        {
            return _transform.TransformPoint(point);
        }

        public Point InverseTransformPoint(Point point)
        {
            return _transform.Inverse.TransformPoint(point);
        }
    }
}
