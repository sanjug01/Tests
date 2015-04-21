using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class GestureRoutedEventPropertiesWrapper : IGestureRoutedEventProperties
    {
        private PointerEvent _pointerEvent;

        public GestureRoutedEventPropertiesWrapper(PointerEvent pointerEvent)
        {
            _pointerEvent = pointerEvent;
        }

        public PointerEventAction Action
        {
            get
            {
                return _pointerEvent.Action;
            }
        }

        public uint Count
        {
            get
            {
                if (_pointerEvent.Action == PointerEventAction.Tapped)
                {
                    return ((TappedEventArgs)_pointerEvent.Args).TapCount;
                }
                else
                {
                    return 0;
                }
            }
        }

        public PointerDeviceType DeviceType
        {
            get
            {
                switch (_pointerEvent.Type)
                {
                    case PointerEventType.TappedEventArgs:
                        return ((TappedEventArgs)_pointerEvent.Args).PointerDeviceType;
                    case PointerEventType.HoldingEventArgs:
                        return ((HoldingEventArgs)_pointerEvent.Args).PointerDeviceType;
                    case PointerEventType.DraggingEventArgs:
                        return ((DraggingEventArgs)_pointerEvent.Args).PointerDeviceType;
                    default:
                        return PointerDeviceType.Mouse;
                }
            }
        }

        public Point Position
        {
            get
            {
                switch (_pointerEvent.Type)
                {
                    case PointerEventType.TappedEventArgs:
                        return ((TappedEventArgs)_pointerEvent.Args).Position;
                    case PointerEventType.HoldingEventArgs:
                        return ((HoldingEventArgs)_pointerEvent.Args).Position;
                    case PointerEventType.DraggingEventArgs:
                        return ((DraggingEventArgs)_pointerEvent.Args).Position;
                    default:
                        return new Point(double.NaN, double.NaN);
                }
            }
        }
    }
}
