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
        ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private System.Double LockRead(ref System.Double parameter)
        {
            System.Double result;
            _rwLock.EnterReadLock();
            result = parameter;
            _rwLock.ExitReadLock();
            return result;
        }

        private void LockWrite(ref System.Double parameter, System.Double value)
        {
            _rwLock.EnterWriteLock();
            parameter = value;
            _rwLock.ExitWriteLock();
        }

        private System.Double _centerX;
        public System.Double CenterX
        {
            get
            {
                return LockRead(ref _centerX);
            }
            set
            {
                LockWrite(ref _centerX, value);
                _transform.CenterX = value;
            }
        }

        private System.Double _centerY;
        public System.Double CenterY
        {
            get
            {
                return LockRead(ref _centerY);
            }
            set
            {
                LockWrite(ref _centerY, value);
                _transform.CenterY = value;
            }
        }

        private System.Double _rotation;
        public System.Double Rotation
        {
            get
            {
                return LockRead(ref _rotation);
            }
            set
            {
                LockWrite(ref _rotation, value);
                _transform.Rotation = value;
            }
        }

        public System.Double _scaleX;
        public System.Double ScaleX
        {
            get
            {
                return LockRead(ref _scaleX);
            }
            set
            {
                LockWrite(ref _scaleX, value);
                _transform.ScaleX = value;
            }
        }

        public System.Double _scaleY;
        public System.Double ScaleY
        {
            get
            {
                return LockRead(ref _scaleY);
            }
            set
            {
                LockWrite(ref _scaleY, value);
                _transform.ScaleY = value;
            }
        }

        public System.Double _skewX;
        public System.Double SkewX
        {
            get
            {
                return LockRead(ref _skewX);
            }
            set
            {
                LockWrite(ref _skewX, value);
                _transform.SkewX = value;
            }
        }

        public System.Double _skewY;
        public System.Double SkewY
        {
            get
            {
                return LockRead(ref _skewY);
            }
            set
            {
                LockWrite(ref _skewY, value);
                _transform.SkewY = value;
            }
        }

        public System.Double _translateX;
        public System.Double TranslateX
        {
            get
            {
                return LockRead(ref _translateX);
            }
            set
            {
                LockWrite(ref _translateX, value);
                _transform.TranslateX = value;
            }
        }

        public System.Double _translateY;
        public System.Double TranslateY
        {
            get
            {
                return LockRead(ref _translateY);
            }
            set
            {
                LockWrite(ref _translateY, value);
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
