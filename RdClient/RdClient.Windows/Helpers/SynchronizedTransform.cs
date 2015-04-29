using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace RdClient.Helpers
{
    public class SynchronizedTransform
    {
        private CompositeTransform _transform;

        private System.Double _centerX;
        public System.Double CenterX
        {
            get
            {
                return _centerX;
            }
            set
            {
                Interlocked.Exchange(ref _centerX, value);
                _transform.CenterX = value;
            }
        }

        private System.Double _centerY;
        public System.Double CenterY
        {
            get
            {
                return _centerY;
            }
            set
            {
                Interlocked.Exchange(ref _centerY, value);
                _transform.CenterY = value;
            }
        }

        private System.Double _rotation;
        public System.Double Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                Interlocked.Exchange(ref _rotation, value);
                _transform.Rotation = value;
            }
        }

        public System.Double _scaleX;
        public System.Double ScaleX
        {
            get
            {
                return _scaleX;
            }
            set
            {
                Interlocked.Exchange(ref _scaleX, value);
                _transform.ScaleX = value;
            }
        }

        public System.Double _scaleY;
        public System.Double ScaleY
        {
            get
            {
                return _scaleY;
            }
            set
            {
                Interlocked.Exchange(ref _scaleY, value);
                _transform.ScaleY = value;
            }
        }

        public System.Double _skewX;
        public System.Double SkewX
        {
            get
            {
                return _skewX;
            }
            set
            {
                Interlocked.Exchange(ref _skewX, value);
                _transform.SkewX = value;
            }
        }

        public System.Double _skewY;
        public System.Double SkewY
        {
            get
            {
                return _skewY;
            }
            set
            {
                Interlocked.Exchange(ref _skewY, value);
                _transform.SkewY = value;
            }
        }

        public System.Double _translateX;
        public System.Double TranslateX
        {
            get
            {
                return _translateX;
            }
            set
            {
                Interlocked.Exchange(ref _translateX, value);
                _transform.TranslateX = value;
            }
        }

        public System.Double _translateY;
        public System.Double TranslateY
        {
            get
            {
                return _translateY;
            }
            set
            {
                Interlocked.Exchange(ref _translateY, value);
                _transform.TranslateY = value;
            }
        }

        public CompositeTransform Transform
        {
            get { return _transform; }
        }

        public void AssignTo(CompositeTransform transform)
        {
            var ignore = transform.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    transform.CenterX = CenterX;
                    transform.CenterY = CenterY;
                    transform.Rotation = Rotation;
                    transform.ScaleX = ScaleX;
                    transform.ScaleY = ScaleY;
                    transform.SkewX = SkewX;
                    transform.SkewY = SkewY;
                    transform.TranslateX = TranslateX;
                    transform.TranslateY = TranslateY;
                });
        }

        public SynchronizedTransform(CompositeTransform transform)
        {
            _transform = transform;
            CenterX = _transform.CenterX;
            CenterY = _transform.CenterY;
            Rotation = _transform.Rotation;
            ScaleX = _transform.ScaleX;
            ScaleY = _transform.ScaleY;
            SkewX = _transform.SkewX;
            SkewY = _transform.SkewY;
            TranslateX = _transform.TranslateX;
            TranslateY = _transform.TranslateY;
        }
    }
}
