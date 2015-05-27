using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerPosition : IPointerPosition
    {
        private IRemoteSessionControl _sessionControl;
        private IExecutionDeferrer _deferrer;

        public event EventHandler<Point> PositionChanged;

        private Point _viewportPosition;
        Point IPointerPosition.ViewportPosition
        {
            get { return _viewportPosition; }
            set
            {
                Point mP = new Point(
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Width, Math.Max(0, value.X)),
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Height, Math.Max(0, value.Y)));

                _viewportPosition = mP;
                _deferrer.DeferToUI(() => {
                    _sessionControl.RenderingPanel.MoveMouseCursor(_viewportPosition);
                    EmitPositionChanged(_viewportPosition);
                });
            }
        }

        Point IPointerPosition.SessionPosition
        {
            get
            {
                return _sessionControl.RenderingPanel.Viewport.SessionPanel.Transform.InverseTransformPoint(_viewportPosition);
            }
        }

        private void EmitPositionChanged(Point position)
        {
            if(PositionChanged != null)
            {
                PositionChanged(this, position);
            }
        }

        void IPointerPosition.Reset(IRemoteSessionControl sessionControl, IExecutionDeferrer deferrer)
        {
            _sessionControl = sessionControl;
            _deferrer = deferrer;
        }
    }
}
