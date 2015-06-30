namespace RdClient.Shared.Models.Viewport
{
    using System;
    using System.ComponentModel;
    using Windows.Foundation;

    public class Viewport : IViewport
    {
        private IViewportPanel _viewportPanel;
        private IViewportPanel _sessionPanel;
        private PropertyChangedEventHandler _propertyChanged;

        private double _maxZoom;
        public double MaxZoom { set { _maxZoom = value; } }

        private double _minZoom;
        public double MinZoom { set { _minZoom = value; } }

        public event EventHandler Changed;

        public Viewport(IViewportPanel sessionPanel, IViewportPanel viewportPanel)
        {
            _viewportPanel = viewportPanel;
            _viewportPanel.PropertyChanged += OnViewportPropertyChanged;
            _sessionPanel = sessionPanel;
            _maxZoom = 4.0;
            _minZoom = 1.0;
            _zoomFactor = 1.0;
        }

        private void OnViewportPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Width" || e.PropertyName == "Height")
            {
                SetPan(this.Offset.X, this.Offset.Y);
                EmitChanged();
            }
        }

        private void EmitChanged()
        {
            if(Changed != null)
            {
                Changed(this, null);
            }
        }

        public IViewportPanel SessionPanel
        {
            get
            {
                return _sessionPanel;
            }
        }

        public Point Offset
        {
            get
            {
                return new Point(-_sessionPanel.Transform.TranslateX, -_sessionPanel.Transform.TranslateY);
            }
        }

        public Size Size
        {
            get
            {
                return new Size(_viewportPanel.Width, _viewportPanel.Height);
            }
        }

        private double _zoomFactor;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        public double ZoomFactor
        {
            get
            {
                return _zoomFactor;
            }
        }

        private double ClampZoomFactor(double zoomFactor)
        {
            return Math.Min(_maxZoom, Math.Max(_minZoom, zoomFactor));
        }

        private void Translate(double dx, double dy)
        {
            double moveX = _sessionPanel.Transform.TranslateX + dx;
            double moveY = _sessionPanel.Transform.TranslateY + dy;

            _sessionPanel.Transform.TranslateX = moveX;
            _sessionPanel.Transform.TranslateY = moveY;
        }

        private void Zoom(double zoomFactor, Point anchorPoint)
        {
            Point oldAnchorPoint = new Point(anchorPoint.X * _sessionPanel.Transform.ScaleX, anchorPoint.Y * _sessionPanel.Transform.ScaleY);
            Point newAnchorPoint = new Point(anchorPoint.X * zoomFactor, anchorPoint.Y * zoomFactor);
            _sessionPanel.Transform.ScaleX = zoomFactor;
            _sessionPanel.Transform.ScaleY = zoomFactor;
            _zoomFactor = zoomFactor;
            Translate(oldAnchorPoint.X - newAnchorPoint.X, oldAnchorPoint.Y - newAnchorPoint.Y);
        }

        public void SetZoom(double zoomFactor, Point anchorPoint)
        {
            zoomFactor = ClampZoomFactor(zoomFactor);
            Zoom(zoomFactor, anchorPoint);
            
            if(Changed != null)
            {
                Changed(this, null);
            }
        }

        public void SetPan(double x, double y)
        {
            double xWiggleRoom = _sessionPanel.Width - _viewportPanel.Width;
            double yWiggleRoom = _sessionPanel.Height - _viewportPanel.Height;

            if(xWiggleRoom < 0)
            {
                x = (_sessionPanel.Width - _viewportPanel.Width) / 2.0;
            }
            else
            {
                x = Math.Min(xWiggleRoom, Math.Max(0, x));
            }

            if(yWiggleRoom < 0)
            {
                y = (_sessionPanel.Height - _viewportPanel.Height) / 2.0;
            }
            else
            {
                y = Math.Min(yWiggleRoom, Math.Max(0, y));
            }

            _sessionPanel.Transform.TranslateX = -x;
            _sessionPanel.Transform.TranslateY = -y;
        }

        public void PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
        {
            double zoomFactor = ClampZoomFactor(_sessionPanel.Transform.ScaleX * scaleFactor);
            Zoom(zoomFactor, anchorPoint);
            Translate(dx, dy);

            if(Changed != null)
            {
                Changed(this, null);
            }
        }

        public void Reset()
        {
            // I have a sneaky suspicion that calling _transform.Inverse will actually try to find an inverse matrix
            // I am optimizng this away by getting an inverse which ignores rotation and thus makes things far simpler
            // DT
            _sessionPanel.Transform.ScaleX = 1.0;
            _sessionPanel.Transform.ScaleY = 1.0;
            _sessionPanel.Transform.TranslateX = 0.0;
            _sessionPanel.Transform.TranslateY = 0.0;
            _zoomFactor = 1.0;
            if (Changed != null)
            {
                Changed(this, null);
            }
        }
    }
}
