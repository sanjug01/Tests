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
        

        public PointerCapture(IPointerPosition pointerPosition, IRemoteSessionControl sessionControl, IRenderingPanel panel, ITimerFactory timerFactory)
        {
            _sessionControl = sessionControl;
            _panel = panel;
            _consumer = new PointerEventDispatcher(timerFactory, sessionControl, pointerPosition);
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
