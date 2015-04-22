using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerPosition : IPointerPosition
    {
        private IRemoteSessionControl _sessionControl;
        private IExecutionDeferrer _deferrer;

        public event EventHandler<Point> PositionChanged;

        private Point _pointerPosition;
        Point IPointerPosition.PointerPosition
        {
            get { return _pointerPosition; }
            set
            {
                Point mP = new Point(
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Width, Math.Max(0, value.X)),
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Height, Math.Max(0, value.Y)));

                _pointerPosition = mP;
                _deferrer.DeferToUI(() => {
                    _sessionControl.RenderingPanel.MoveMouseCursor(_pointerPosition);
                    EmitPositionChanged(_pointerPosition);
                });
            }
        }

        private void EmitPositionChanged(Point position)
        {
            if(PositionChanged != null)
            {
                PositionChanged(this, position);
            }
        }

        public void Reset(IRemoteSessionControl sessionControl, IExecutionDeferrer deferrer)
        {
            _sessionControl = sessionControl;
            _deferrer = deferrer;
        }
    }
}
