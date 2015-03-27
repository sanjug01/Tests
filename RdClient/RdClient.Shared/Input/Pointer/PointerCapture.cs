using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Input
{
    public class PointerCapture : IPointerCapture, IPointerManipulator
    {
        private IExecutionDeferrer _deferrer;
        private IRemoteSessionControl _control;
        private IRenderingPanel _panel;
        private IPointerEventConsumer _consumer;

        public ConsumptionMode ConsumptionMode
        {
            set { _consumer.ConsumptionMode = value; }
        }

        public PointerCapture(IExecutionDeferrer deferrer, IRemoteSessionControl control, IRenderingPanel panel)
        {
            _deferrer = deferrer;
            _control = control;
            _panel = panel;
            _consumer = new PointerEventDispatcher(new WinrtThreadPoolTimer(), this, panel);
            this.ConsumptionMode = ConsumptionMode.Pointer;
        }

        public void OnPointerChanged(object sender, PointerEventArgs args)
        {
            _consumer.ConsumeEvent(args.PointerEvent);
        }

        public void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args)
        {
            throw new System.NotImplementedException();
        }

        public void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {
            Contract.Requires(null != args.Buffer);

            _deferrer.DeferToUI(() =>
            {
                ImageSource image = MouseCursorShape.ByteArrayToBitmap(args.Buffer, args.Width, args.Height);
                MouseCursorShape cursor = new MouseCursorShape(new Point(args.XHotspot, args.YHotspot), image);
                this._panel.ChangeMouseCursorShape(cursor);
            });
        }

        public double MouseAcceleration { get { return (double)1.4; } }

        private Point _mousePosition;
        public Point MousePosition 
        {
            get { return _mousePosition; }
            set 
            {
                _mousePosition.X = Math.Max(0.0, Math.Min(value.X, _panel.Viewport.Size.Width));
                _mousePosition.Y = Math.Max(0.0, Math.Min(value.Y, _panel.Viewport.Size.Height));

                _deferrer.DeferToUI(() => this._panel.MoveMouseCursor(_mousePosition));
            }
        }

        public void SendMouseAction(MouseEventType eventType)
        {
            this._control.SendMouseAction(new MouseAction(eventType, MousePosition));
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            this._control.SendMouseWheel(delta, isHorizontal);
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            this._control.SendTouchAction(type, contactId, position, frameTime);
        }
    }
}
