namespace RdClient.Controls
{
    using RdClient.Helpers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    sealed class RenderingPanelViewport : MutableObject, IViewport
    {
        private static readonly Point _origin = new Point();

        private readonly RemoteSessionPanel _sessionPanel;
        private readonly RenderingPanel _renderingPanel;
        private readonly SynchronizedTransform _transformation;

        private Size _size;
        private Point _offset;
        private double _zoomFactor;

        public RenderingPanelViewport(RemoteSessionPanel sessionPanel, RenderingPanel renderingPanel,
            CompositeTransform transformation)
        {
            Contract.Assert(null != sessionPanel);
            Contract.Assert(null != renderingPanel);

            _sessionPanel = sessionPanel;
            _renderingPanel = renderingPanel;

            _size = new Size();
            _offset = new Point();
            _zoomFactor = 1.0;

            _sessionPanel.SizeChanged += this.OnSizeChanged;
            _transformation = new SynchronizedTransform(transformation);
        }

        Size IViewport.Size
        {
            get { return _size; }
        }

        Point IViewport.Offset
        {
            get { return _offset; }
        }

        double IViewport.ZoomFactor
        {   
            get { return _zoomFactor; }
        }

        Point IViewport.TransformPoint(Point point)
        {
            Point transformed = new Point(point.X, point.Y);           

            transformed.X *= 1.0 / _transformation.ScaleX;
            transformed.Y *= 1.0 / _transformation.ScaleY;
            transformed.X -= _transformation.TranslateX;
            transformed.Y -= _transformation.TranslateY;

            return transformed;
        }

        void IViewport.Set(double zoomFactor, Point anchorPoint)
        {
            zoomFactor = AdjustZoomFactor(zoomFactor);

            CompositeTransform newTransform = new CompositeTransform()
            {
                ScaleX = zoomFactor,
                ScaleY = zoomFactor,
                TranslateX = _transformation.TranslateX,
                TranslateY = _transformation.TranslateY
            };

            Point translatedAnchorPoint = _transformation.Transform.TransformPoint(anchorPoint);
            Point newTranslatedAnchorPoint = newTransform.TransformPoint(anchorPoint);
            Point newShift = new Point(newTranslatedAnchorPoint.X - translatedAnchorPoint.X, newTranslatedAnchorPoint.Y - translatedAnchorPoint.Y);
            newTransform.TranslateX = _transformation.TranslateX - newShift.X;
            newTransform.TranslateY = _transformation.TranslateY - newShift.Y;

            AdjustViewport(newTransform);
        }

        void IViewport.PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
        {
            //
            // Get the position of the anchor point (that is is viewport coordinates) in the rendering panel.
            //
            Point translatedAnchorPoint = _transformation.Transform.TransformPoint(anchorPoint);

            //
            // Calculate the new scale and adjust it to the range 1.0-4.0.
            //
            double newScale = AdjustZoomFactor(_transformation.ScaleX * scaleFactor);

            //
            // Construct a new transformation with the new scale and current translation (offset) and use it to get the position
            // of the anchor point using new scale.
            //
            CompositeTransform newTransform = new CompositeTransform()
            {
                ScaleX = newScale, ScaleY = newScale,
                TranslateX = _transformation.TranslateX, TranslateY = _transformation.TranslateY
            };
            
            //
            // Align the current and new anchor points and move the new anchor point the specified distance (dx/dy).
            //
            Point newTranslatedAnchorPoint = newTransform.TransformPoint(anchorPoint);
            Point newShift = new Point(newTranslatedAnchorPoint.X - translatedAnchorPoint.X, newTranslatedAnchorPoint.Y - translatedAnchorPoint.Y);
            
            newShift.X -= dx;
            newShift.Y -= dy;

            newTransform.TranslateX = _transformation.TranslateX - newShift.X;
            newTransform.TranslateY = _transformation.TranslateY - newShift.Y;

            AdjustViewport(newTransform);
        }

        private void AdjustViewport(CompositeTransform newTransform)
        {
            //
            // Adjust the viewport so it does not go outside of the rendering panel.
            //
            double epsilonHorizontal = Math.Max(0, (_size.Width - _renderingPanel.Width));
            double epsilonVertical = Math.Max(0, (_size.Height - _renderingPanel.Height));

            Rect renderingRect = new Rect(_origin, new Size(_renderingPanel.Width + epsilonHorizontal, _renderingPanel.Height + epsilonVertical));
            Rect viewportRect = new Rect(new Point(-newTransform.TranslateX, -newTransform.TranslateY), new Size(_size.Width * newTransform.ScaleX, _size.Height * newTransform.ScaleY));

            if (viewportRect.Left <= renderingRect.Left)
            {
                newTransform.TranslateX -= renderingRect.Left - viewportRect.Left - epsilonHorizontal / 2.0;
            }
            else if (viewportRect.Right > renderingRect.Right)
            {
                newTransform.TranslateX += viewportRect.Right - renderingRect.Right + epsilonHorizontal / 2.0;
            }

            if (viewportRect.Top <= renderingRect.Top)
            {
                newTransform.TranslateY -= renderingRect.Top - viewportRect.Top - epsilonVertical / 2.0;
            }
            else if (viewportRect.Bottom > renderingRect.Bottom)
            {
                newTransform.TranslateY += viewportRect.Bottom - renderingRect.Bottom + epsilonVertical / 2.0;
            }

            //
            // Round the translate transfortmation down to avoid sub-pixel rendering of the final position.
            //
            newTransform.TranslateX = Math.Round(newTransform.TranslateX);
            newTransform.TranslateY = Math.Round(newTransform.TranslateY);

            _transformation.ScaleX = newTransform.ScaleX;
            _transformation.ScaleY = newTransform.ScaleY;
            if (_renderingPanel.MouseScaleTransform != null)
            {
                _renderingPanel.MouseScaleTransform.ScaleX = newTransform.ScaleX;
                _renderingPanel.MouseScaleTransform.ScaleY = newTransform.ScaleY;

            }
            _transformation.TranslateX = newTransform.TranslateX;
            _transformation.TranslateY = newTransform.TranslateY;

            this.ZoomFactor = _transformation.ScaleX;
            this.Offset = new Point(_transformation.TranslateX, _transformation.TranslateY);
        }

        private static double AdjustZoomFactor(double desiredZoomFactor)
        {
            if (desiredZoomFactor > 4.0)
                desiredZoomFactor = 4.0;
            else if (desiredZoomFactor < 1.0)
                desiredZoomFactor = 1.0;

            return desiredZoomFactor;
        }

        private Size Size
        {
            set { this.SetProperty(ref _size, value); }
        }

        private Point Offset
        {
            set { this.SetProperty(ref _offset, value); }
        }

        private double ZoomFactor
        {
            set { this.SetProperty(ref _zoomFactor, value); }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // Adjust the viewport position to its new size;
            //

            this.Size = e.NewSize;
            AdjustViewport(_transformation.Transform);
        }        
    }
}
