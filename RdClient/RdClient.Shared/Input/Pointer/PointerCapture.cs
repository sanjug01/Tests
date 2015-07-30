using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
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
        private MouseCursorShapes _mouseCursorShapes;

        private IConsumptionModeTracker _consumptionMode;
        public IConsumptionModeTracker ConsumptionMode
        {
            get { return _consumptionMode; }
        }

        //_pointerMode = new PointerModeConsumer(
        //        new RdDispatcherTimer(timerFactory.CreateTimer(), dispatcher), 
        //        new PointerModeControl(sessionControl, pointerPosition));
        //    _multiTouchMode = new MultiTouchConsumer(sessionControl, pointerPosition);
        //_directMode = new DirectModeConsumer(new DirectModeControl(sessionControl, pointerPosition), pointerPosition);
            
        //    _consumers[PointerDeviceType.Mouse] = new MouseModeConsumer(sessionControl, pointerPosition);
        //_consumers[PointerDeviceType.Pen] = new MouseModeConsumer(sessionControl, pointerPosition);
        //_consumers[PointerDeviceType.Touch] = _pointerMode;

        public PointerCapture(IPointerPosition pointerPosition, IRemoteSessionControl sessionControl, IRenderingPanel panel, ITimerFactory timerFactory, IDeferredExecution deferrer)
        {
            _sessionControl = sessionControl;
            _panel = panel;
            PointerEventDispatcher dispatcher = new PointerEventDispatcher(
                timerFactory,
                new PointerDeviceDispatcher(
                    new PointerModeConsumer(new RdDispatcherTimer(timerFactory.CreateTimer(), deferrer), new PointerModeControl(sessionControl, pointerPosition)),
                    new MultiTouchConsumer(sessionControl, pointerPosition),
                    new DirectModeConsumer(new DirectModeControl(sessionControl, pointerPosition), pointerPosition),
                    new MouseModeConsumer(sessionControl, pointerPosition)),
                new PointerVisibilityConsumer(sessionControl.RenderingPanel));
            _consumer = dispatcher;
            _consumptionMode = new ConsumptionModeTracker() { ConsumptionMode = ConsumptionModeType.Pointer };
            _consumptionMode.ConsumptionModeChanged += (s, o) => dispatcher.SetConsumptionMode(o);
            _mouseCursorShapes = new MouseCursorShapes(new CursorEncoder());
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
            ImageSource shape = _mouseCursorShapes.GetImageSource(args.Buffer, args.Width, args.Height);
            Point hotspot = new Point(args.XHotspot, args.YHotspot);
            this._panel.ChangeMouseCursorShape(shape, hotspot);
        }

        void IPointerCapture.ChangeInputMode(InputMode inputMode)
        {
            switch (inputMode)
            {
                case InputMode.Mouse:
                    _consumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;
                    break;
                case InputMode.Touch:
                    if(_multiTouchEnabled)
                    {
                        _consumptionMode.ConsumptionMode = ConsumptionModeType.MultiTouch;
                    }
                    else
                    {
                        _consumptionMode.ConsumptionMode = ConsumptionModeType.DirectTouch;
                    }
                    break;
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
