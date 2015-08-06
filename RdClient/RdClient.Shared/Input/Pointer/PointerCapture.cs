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
        private InputMode _inputMode;
        private bool _multiTouchEnabled;
        private MouseCursorShapes _mouseCursorShapes;

        private IConsumptionModeTracker _consumptionMode;
        public IConsumptionModeTracker ConsumptionMode
        {
            get { return _consumptionMode; }
        }

        private IInputDeviceTracker _inputDeviceTracker;
        public IInputDeviceTracker InputDevice
        {
            get { return _inputDeviceTracker; }
        }

        public PointerCapture(IPointerPosition pointerPosition, IRemoteSessionControl sessionControl, IRenderingPanel panel, ITimerFactory timerFactory, IDeferredExecution deferrer)
        {
            _sessionControl = sessionControl;
            _panel = panel;

            MouseModeConsumer mouseModeConsumer = new MouseModeConsumer(sessionControl, pointerPosition);

            DirectModeControl directModeControl = new DirectModeControl(sessionControl, pointerPosition);
            DirectModeConsumer directModeConsumer = new DirectModeConsumer(directModeControl, pointerPosition);

            MultiTouchConsumer multiTouchConsumer = new MultiTouchConsumer(sessionControl, pointerPosition);

            RdDispatcherTimer rdDispatcherTimer = new RdDispatcherTimer(timerFactory.CreateTimer(), deferrer);
            PointerModeControl pointerModeControl = new PointerModeControl(sessionControl, pointerPosition);
            PointerModeConsumer pointerModeConsumer = new PointerModeConsumer(rdDispatcherTimer, pointerModeControl);

            PointerVisibilityConsumer pointerVisibilityConsumer = new PointerVisibilityConsumer(sessionControl.RenderingPanel);

            _inputDeviceTracker = new InputDeviceTracker();
            PointerDeviceDispatcher pointerDeviceDispatcher = new PointerDeviceDispatcher(
                    pointerModeConsumer,
                    multiTouchConsumer,
                    directModeConsumer,
                    mouseModeConsumer,
                    _inputDeviceTracker);

            PointerEventDispatcher pointerEventDispatcher = new PointerEventDispatcher(
                pointerDeviceDispatcher,
                pointerVisibilityConsumer);

            _consumer = pointerEventDispatcher;
            _inputMode = InputMode.Mouse;
            _consumptionMode = new ConsumptionModeTracker() { ConsumptionMode = ConsumptionModeType.Pointer };
            _consumptionMode.ConsumptionModeChanged += (s, o) => pointerEventDispatcher.SetConsumptionMode(o);
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

        InputMode IPointerCapture.InputMode
        {
            get { return _inputMode; }

            set
            {
                if(value != _inputMode)
                {
                    _inputMode = value;

                    switch (_inputMode)
                    {
                        case InputMode.Mouse:
                            _consumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;
                            break;

                        case InputMode.Touch:
                            _consumptionMode.ConsumptionMode =_multiTouchEnabled ?
                                ConsumptionModeType.MultiTouch : ConsumptionModeType.DirectTouch;
                            break;
                    }
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
