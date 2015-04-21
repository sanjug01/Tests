using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerCapture : IPointerCapture, IPointerPosition
    {
        private IExecutionDeferrer _deferrer;
        private IRemoteSessionControl _sessionControl;
        private IRenderingPanel _panel;
        private IPointerEventConsumer _consumer;
        private bool _multiTouchEnabled;

        private ConsumptionMode _consumptionMode;
        private ConsumptionMode ConsumptionMode
        {
            get { return _consumptionMode; }
            set
            {
                _consumer.ConsumptionMode = value;
                _consumptionMode = value;
            }
        }

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
                _deferrer.DeferToUI(() => _sessionControl.RenderingPanel.MoveMouseCursor(_pointerPosition));
            }
        }

        public PointerCapture(IExecutionDeferrer deferrer, IRemoteSessionControl sessionControl, IRenderingPanel panel, ITimerFactory timerFactory)
        {
            _deferrer = deferrer;
            _sessionControl = sessionControl;
            _panel = panel;
            _consumer = new PointerEventDispatcher(timerFactory, sessionControl, this as IPointerPosition);
            this.ConsumptionMode = ConsumptionMode.Pointer;
        }

        void IPointerCapture.OnPointerChanged(object sender, IPointerEventBase e)
        {
            _consumer.Consume(e);
        }

        void IPointerCapture.OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args)
        {
            throw new System.NotImplementedException();
        }

        void IPointerCapture.OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {
            Contract.Requires(null != args.Buffer);
            ImageSource image = MouseCursorShape.ByteArrayToBitmap(args.Buffer, args.Width, args.Height);
            MouseCursorShape cursor = new MouseCursorShape(new Point(args.XHotspot, args.YHotspot), image);
            this._panel.ChangeMouseCursorShape(cursor);
        }

        void IPointerCapture.OnMouseModeChanged(object sender, EventArgs e)
        {
            if (_multiTouchEnabled)
            {
                if (ConsumptionMode == ConsumptionMode.MultiTouch)
                {
                    ConsumptionMode = ConsumptionMode.Pointer;
                }
                else
                {
                    ConsumptionMode = ConsumptionMode.MultiTouch;
                }
            }
            else
            {
                if (ConsumptionMode == ConsumptionMode.DirectTouch)
                {
                    ConsumptionMode = ConsumptionMode.Pointer;
                }
                else
                {
                    ConsumptionMode = ConsumptionMode.DirectTouch;
                }
            }
        }

        public void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs args)
        {
            _multiTouchEnabled = args.MultiTouchEnabled;
            
            if(_multiTouchEnabled)
            {
                if(ConsumptionMode == ConsumptionMode.DirectTouch)
                {
                    ConsumptionMode = ConsumptionMode.MultiTouch;
                }
            }
            else
            {
                if(ConsumptionMode == ConsumptionMode.MultiTouch)
                {
                    ConsumptionMode = ConsumptionMode.DirectTouch;
                }
            }
        }

    }
}
