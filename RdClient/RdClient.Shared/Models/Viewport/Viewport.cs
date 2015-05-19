using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Models.Viewport
{
    public class Viewport : IViewport
    {
        private IViewportPanel _viewportPanel;
        private IViewportPanel _sessionPanel;

        private double _maxZoom;
        public double MaxZoom { set { _maxZoom = value; } }

        private double _minZoom;
        public double MinZoom { set { _minZoom = value; } }

        public Viewport(IViewportPanel sessionPanel, IViewportPanel viewportPanel)
        {
            _viewportPanel = viewportPanel;
            _sessionPanel = sessionPanel;
            _maxZoom = 4.0;
            _minZoom = 1.0;
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

        public event PropertyChangedEventHandler PropertyChanged;

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

        public static double ClampIntervals(double left1, double right1, double left2, double right2, double d)
        {
            if (left1 + d > left2)
                return left2 - left1;
            else if (right1 + d < right2)
                return right2 - right1;
            else
                return d;            
        }

        private void Translate(double dx, double dy)
        {
            Rect sessionRect = new Rect(new Point(_sessionPanel.Transform.TranslateX, _sessionPanel.Transform.TranslateY), new Size(_sessionPanel.Width, _sessionPanel.Height));
            Rect viewportRect = new Rect(new Point(0, 0), new Size(_viewportPanel.Width, _viewportPanel.Height));

            Debug.WriteLine("translateX {0} translateY{1}", _sessionPanel.Transform.TranslateX, _sessionPanel.Transform.TranslateY);
            Debug.WriteLine("dx {0} dy {1}", dx, dy);
            Debug.WriteLine("session left {0} right {1} top {2} bottom {3}", sessionRect.Left, sessionRect.Right, sessionRect.Top, sessionRect.Bottom);
            Debug.WriteLine("viewport left {0} right {1} top {2} bottom {3}", viewportRect.Left, viewportRect.Right, viewportRect.Top, viewportRect.Bottom);
            Debug.WriteLine("clamped x: {0} clamped y: {1}", 
                ClampIntervals(sessionRect.Left, sessionRect.Right, viewportRect.Left, viewportRect.Right, _sessionPanel.Transform.TranslateX + dx),
                ClampIntervals(sessionRect.Top, sessionRect.Bottom, viewportRect.Top, viewportRect.Bottom, _sessionPanel.Transform.TranslateY + dy));

            if (viewportRect.Width < sessionRect.Width)
            {
                _sessionPanel.Transform.TranslateX += ClampIntervals(sessionRect.Left, sessionRect.Right, viewportRect.Left, viewportRect.Right, dx);
            }
            else if (viewportRect.Width > sessionRect.Width)
            {
                _sessionPanel.Transform.TranslateX = (viewportRect.Width - sessionRect.Width) / 2.0;
            }

            if(viewportRect.Height < sessionRect.Height)
            {
                _sessionPanel.Transform.TranslateY += ClampIntervals(sessionRect.Top, sessionRect.Bottom, viewportRect.Top, viewportRect.Bottom, dy);
            }
            else if(viewportRect.Height > sessionRect.Height)
            {
                _sessionPanel.Transform.TranslateY = (viewportRect.Height - sessionRect.Height) / 2.0;
            }

            Debug.WriteLine("translateX {0} translateY{1}", _sessionPanel.Transform.TranslateX, _sessionPanel.Transform.TranslateY);
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

        public void Set(double zoomFactor, Point anchorPoint)
        {
            zoomFactor = ClampZoomFactor(zoomFactor);
            Zoom(zoomFactor, anchorPoint);
        }

        public void PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
        {
            double zoomFactor = ClampZoomFactor(_sessionPanel.Transform.ScaleX * scaleFactor);
            Zoom(zoomFactor, anchorPoint);
            Translate(dx, dy);
        }

    }
}
