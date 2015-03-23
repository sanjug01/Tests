namespace RdClient.Controls
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
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
        private readonly SwapChainPanel _renderingPanel;
        private readonly Storyboard _storyboard;
        private readonly CompositeTransform _transformation;

        private Size _size;
        private Storyboard _activeStoryboard;

        public RenderingPanelViewport(RemoteSessionPanel sessionPanel, SwapChainPanel renderingPanel,
            CompositeTransform transformation)
        {
            Contract.Assert(null != sessionPanel);
            Contract.Assert(null != renderingPanel);

            _sessionPanel = sessionPanel;
            _size = new Size(_sessionPanel.ActualWidth, _sessionPanel.ActualHeight);
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

        private Size Size
        {
            set { this.SetProperty(ref _size, value); }
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
            double newScale = _transformation.ScaleX * scaleFactor;

            if (newScale > 4.0)
                newScale = 4.0;
            else if (newScale < 1.0)
                newScale = 1.0;
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
            Rect newView = newTransform.TransformBounds(new Rect(_origin, _size));

            if (newView.Left > 0.0)
                newTransform.TranslateX -= newView.Left;
            else if (newView.Right < _size.Width)
                newTransform.TranslateX += _size.Width - newView.Right;

            if (newView.Top > 0.0)
                newTransform.TranslateY -= newView.Top;
            else if (newView.Bottom < _size.Height)
                newTransform.TranslateY += _size.Height - newView.Bottom;
            //
            // Round the translate transfortmation down to avoid sub-pixel rendering of the final position.
            //
            newTransform.TranslateX = Math.Floor(newTransform.TranslateX);
            newTransform.TranslateY = Math.Floor(newTransform.TranslateY);
            //
            // Animate the transition
            //
            if(durationMilliseconds < 0.02)
            {
                //
                // Too fast for animation, just update the transformation.
                //
                _transformation.ScaleX = newTransform.ScaleX;
                _transformation.ScaleY = newTransform.ScaleY;
                _transformation.TranslateX = newTransform.TranslateX;
                _transformation.TranslateY = newTransform.TranslateY;
            }
            else
            {
                DoubleAnimation
                    animationTranslateX = new DoubleAnimation() { From = _transformation.TranslateX, To = newTransform.TranslateX },
                    animationTranslateY = new DoubleAnimation() { From = _transformation.TranslateY, To = newTransform.TranslateY },
                    animationScaleX = new DoubleAnimation() { From = _transformation.ScaleX, To = newTransform.ScaleX },
                    animationScaleY = new DoubleAnimation() { From = _transformation.ScaleY, To = newTransform.ScaleY };

                _storyboard.Children.Add(animationTranslateX);
                _storyboard.Children.Add(animationTranslateY);
                _storyboard.Children.Add(animationScaleX);
                _storyboard.Children.Add(animationScaleY);
                _storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds));

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
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // TODO: adjust the viewport position to its new size;
            //
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
        }
    }
}
