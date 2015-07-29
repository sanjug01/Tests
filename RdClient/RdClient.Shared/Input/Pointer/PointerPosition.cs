using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerPosition : IPointerPosition
    {
        private IRenderingPanel _renderingPanel;
        private ISynchronizedDeferrer _deferrer;

        public event EventHandler<Point> PositionChanged;

        private Point _viewportPosition;
        Point IPointerPosition.ViewportPosition
        {
            get { return _viewportPosition; }
            set
            {
                Point mP = new Point(
                    Math.Min(_renderingPanel.Viewport.Size.Width, Math.Max(0, value.X)),
                    Math.Min(_renderingPanel.Viewport.Size.Height, Math.Max(0, value.Y)));

                _viewportPosition = mP;
                _deferrer.DeferToUI(() => {
                    _renderingPanel.MoveMouseCursor(_viewportPosition);
                    EmitPositionChanged(_viewportPosition);
                });
            }
        }

        Point IPointerPosition.SessionPosition
        {
            get
            {
                return _renderingPanel.Viewport.SessionPanel.Transform.InverseTransformPoint(_viewportPosition);
            }
        }

        private void EmitPositionChanged(Point position)
        {
            if(PositionChanged != null)
            {
                PositionChanged(this, position);
            }
        }

        void IPointerPosition.Reset(IRenderingPanel sessionControl, ISynchronizedDeferrer deferrer)
        {
            _renderingPanel = sessionControl;
            _deferrer = deferrer;

            Point center = new Point(sessionControl.Viewport.SessionPanel.Width / 2.0, sessionControl.Viewport.SessionPanel.Height / 2.0);
            ((IPointerPosition)this).ViewportPosition = center;

            _renderingPanel.Viewport.Changed -= OnViewportChanged;

            _renderingPanel.Viewport.Changed += OnViewportChanged;
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            _renderingPanel.ScaleMouseCursor(_renderingPanel.Viewport.ZoomFactor);
        }
    }
}
