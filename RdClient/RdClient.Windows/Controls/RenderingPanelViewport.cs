namespace RdClient.Controls
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    sealed class RenderingPanelViewport : MutableObject, IViewport
    {
        private static readonly Point _origin = new Point();

        private readonly RemoteSessionPanel _sessionPanel;
        private readonly RenderingPanel _renderingPanel;
        private readonly Storyboard _storyboard;
        private readonly CompositeTransform _transformation;

        private Size _size;
        private Point _offset;
        private double _zoomFactor;
        private Storyboard _activeStoryboard;

        public RenderingPanelViewport(RemoteSessionPanel sessionPanel, RenderingPanel renderingPanel,
            CompositeTransform transformation)
        {
            Contract.Assert(null != sessionPanel);
            Contract.Assert(null != renderingPanel);

            _sessionPanel = sessionPanel;
            _size = new Size(_sessionPanel.ActualWidth, _sessionPanel.ActualHeight);
            _offset = new Point();
            _zoomFactor = 1.0;
            _sessionPanel.SizeChanged += this.OnSizeChanged;
            _renderingPanel = renderingPanel;
            _storyboard = new Storyboard();
            _storyboard.Completed += this.OnAnimationCompleted;
            _transformation = transformation;
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
            return _transformation.Inverse.TransformPoint(point);
        }

        void IViewport.Set(double zoomFactor, Size offset, bool animated)
        {
            zoomFactor = AdjustZoomFactor(zoomFactor);

            CompositeTransform newTransform = new CompositeTransform()
            {
                ScaleX = zoomFactor,
                ScaleY = zoomFactor,
            };

            Point newSize = newTransform.TransformPoint(new Point(_size.Width, _size.Height));

            if (offset.Width + _size.Width > newSize.X)
                offset.Width = Math.Floor(newSize.X - _size.Width);
            else
                offset.Width = Math.Floor(offset.Width);

            if (offset.Height + _size.Height > newSize.Y)
                offset.Height = Math.Floor(newSize.Y - _size.Height);
            else
                offset.Height = Math.Floor(offset.Height);

            if(animated)
            {
                Animate(zoomFactor, offset.Width, offset.Height, 0.1);
            }
            else
            {
                _transformation.ScaleX = zoomFactor;
                _transformation.ScaleY = zoomFactor;
                _renderingPanel.MouseScaleTransform.ScaleX = zoomFactor;
                _renderingPanel.MouseScaleTransform.ScaleY = zoomFactor;
                _transformation.TranslateX = -offset.Width;
                _transformation.TranslateY = -offset.Height;
                this.ZoomFactor = zoomFactor;
                this.Offset = new Point(offset.Width, offset.Height);
            }
        }

        void IViewport.PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor, double durationMilliseconds)
        {
            //
            // If the animation is running, stop it.
            //
            if(null != _activeStoryboard)
            {
                ConcludeAnimation(_activeStoryboard);
                _activeStoryboard = null;
            }
            //
            // Calculate the new scale and adjust it to the range 1.0-4.0.
            //
            double newScale = AdjustZoomFactor(_transformation.ScaleX * scaleFactor);
            //
            // Get the position of the anchor point (that is is viewport coordinates) in the rendering panel.
            //
            Point translatedAnchorPoint = _transformation.TransformPoint(anchorPoint);
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

            //
            // Adjust the viewport so it does not go outside of the rendering panel.
            //
            Rect oldView = new Rect(_origin, _size);
            Rect newView = newTransform.TransformBounds(oldView);

            if (newView.Left > oldView.Left)
                newTransform.TranslateX -= newView.Left;
            else if (newView.Right < oldView.Right)
                newTransform.TranslateX += oldView.Right - newView.Right;

            if (newView.Top > oldView.Top)
                newTransform.TranslateY -= newView.Top;
            else if (newView.Bottom < oldView.Bottom)
                newTransform.TranslateY += oldView.Bottom - newView.Bottom;
            //
            // Round the translate transfortmation down to avoid sub-pixel rendering of the final position.
            //
            newTransform.TranslateX = Math.Round(newTransform.TranslateX);
            newTransform.TranslateY = Math.Round(newTransform.TranslateY);

            //
            // Animate the transition
            //
            if (durationMilliseconds < 0.02)
            {
                //
                // Too fast for animation, just update the transformation.
                //

                _transformation.ScaleX = newTransform.ScaleX;
                _transformation.ScaleY = newTransform.ScaleY;
                _renderingPanel.MouseScaleTransform.ScaleX = newTransform.ScaleX;
                _renderingPanel.MouseScaleTransform.ScaleY = newTransform.ScaleY;
                _transformation.TranslateX = newTransform.TranslateX;
                _transformation.TranslateY = newTransform.TranslateY;

                this.ZoomFactor = _transformation.ScaleX;
                this.Offset = new Point(_transformation.TranslateX, _transformation.TranslateY);
            }
            else
            {
                Animate(newTransform.ScaleX, -newTransform.TranslateX, -newTransform.TranslateY, durationMilliseconds);
            }
        }

        private static double AdjustZoomFactor(double desiredZoomFactor)
        {
            if (desiredZoomFactor > 4.0)
                desiredZoomFactor = 4.0;
            else if (desiredZoomFactor < 1.0)
                desiredZoomFactor = 1.0;

            return desiredZoomFactor;
        }

        private void Animate(double zoomFactor, double offsetX, double offsetY, double duration)
        {
            DoubleAnimation
                animationTranslateX = new DoubleAnimation() { From = _transformation.TranslateX, To = -offsetX },
                animationTranslateY = new DoubleAnimation() { From = _transformation.TranslateY, To = -offsetY },
                animationScaleX = new DoubleAnimation() { From = _transformation.ScaleX, To = zoomFactor },
                animationScaleY = new DoubleAnimation() { From = _transformation.ScaleY, To = zoomFactor };

            _storyboard.Children.Add(animationTranslateX);
            _storyboard.Children.Add(animationTranslateY);
            _storyboard.Children.Add(animationScaleX);
            _storyboard.Children.Add(animationScaleY);
            _storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(duration));

            Storyboard.SetTarget(animationTranslateX, _transformation);
            Storyboard.SetTargetProperty(animationTranslateX, "TranslateX");
            Storyboard.SetTarget(animationTranslateY, _transformation);
            Storyboard.SetTargetProperty(animationTranslateY, "TranslateY");

            Storyboard.SetTarget(animationScaleX, _transformation);
            Storyboard.SetTargetProperty(animationScaleX, "ScaleX");
            Storyboard.SetTarget(animationScaleY, _transformation);
            Storyboard.SetTargetProperty(animationScaleY, "ScaleY");

            Contract.Assert(null == _activeStoryboard);
            _activeStoryboard = _storyboard;
            _activeStoryboard.Begin();
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
            if(null != _activeStoryboard)
            {
                ConcludeAnimation(_activeStoryboard);
                _activeStoryboard = null;
            }

            this.Size = e.NewSize;
        }

        private void OnAnimationCompleted(object sender, object e)
        {
            Contract.Assert(object.ReferenceEquals(_activeStoryboard, _storyboard));
            ConcludeAnimation(_activeStoryboard);
            _activeStoryboard = null;
        }

        private void ConcludeAnimation(Storyboard storyboard)
        {
            //
            // Save the current animated values.
            //
            double
                animatedScale = _transformation.ScaleX,
                animatedTranslateX = _transformation.TranslateX,
                animatedTranslateY = _transformation.TranslateY;
            //
            // Stop the storyboard and remove all animations from it.
            //
            storyboard.Stop();
            storyboard.Children.Clear();
            //
            // Restore saved transformation values.
            //
            _transformation.ScaleX = animatedScale;
            _transformation.TranslateX = animatedTranslateX;
            _transformation.TranslateY = animatedTranslateY;
            //
            // Report the new zoom factor and offset by setting the properties of the IViewport object.
            //
            this.ZoomFactor = _transformation.ScaleX;
            this.Offset = new Point(_transformation.TranslateX, _transformation.TranslateY);
        }
    }
}
