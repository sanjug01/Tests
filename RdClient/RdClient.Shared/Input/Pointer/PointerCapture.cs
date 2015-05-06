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
    public class PointerCapture : IPointerCapture
    {
        private IRemoteSessionControl _sessionControl;
        private IRenderingPanel _panel;
        private IPointerEventConsumer _consumer;
        private bool _multiTouchEnabled;

        private IConsumptionMode _consumptionMode;
        public IConsumptionMode ConsumptionMode
        {
            get { return _consumptionMode; }
        }


        public PointerCapture(IPointerPosition pointerPosition, IRemoteSessionControl sessionControl, IRenderingPanel panel, ITimerFactory timerFactory)
        {
            _sessionControl = sessionControl;
            _panel = panel;
            PointerEventDispatcher dispatcher = new PointerEventDispatcher(timerFactory, sessionControl, pointerPosition);
            _consumer = dispatcher;
            _consumptionMode = new ConsumptionModeTracker() { ConsumptionMode = ConsumptionModeType.Pointer };
            _consumptionMode.ConsumptionModeChanged += (s, o) => dispatcher.ConsumptionMode = o;
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
                if (_consumptionMode.ConsumptionMode == ConsumptionModeType.MultiTouch)
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;
                }
                else
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.MultiTouch;
                }
            }
            else
            {
                if (_consumptionMode.ConsumptionMode == ConsumptionModeType.DirectTouch)
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;
                }
                else
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.DirectTouch;
                }
            }
        }

        void IPointerCapture.OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs args)
        {
            _multiTouchEnabled = args.MultiTouchEnabled;
            
            if(_multiTouchEnabled)
            {
                if(_consumptionMode.ConsumptionMode == ConsumptionModeType.DirectTouch)
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.MultiTouch;
                }
            }
            else
            {
                if(_consumptionMode.ConsumptionMode == ConsumptionModeType.MultiTouch)
                {
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.DirectTouch;
                }
            }
        }
    }
}
