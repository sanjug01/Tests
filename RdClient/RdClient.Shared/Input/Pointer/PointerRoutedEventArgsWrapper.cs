using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerRoutedEventArgsWrapper : IPointerRoutedEventProperties
    {
        private readonly PointerEvent _pointerEvent;

        public PointerEventAction Action { get { return _pointerEvent.Action; } }

        public PointerDeviceType DeviceType
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.PointerDevice.PointerDeviceType;
            }
        }

        uint IPointerRoutedEventProperties.PointerId
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.PointerId;
            }
        }

        Point IPointerEventBase.Position
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Position;
            }
        }

        ulong IPointerRoutedEventProperties.Timestamp
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Timestamp;
            }
        }

        bool IPointerRoutedEventProperties.LeftButton
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.IsLeftButtonPressed;
            }
        }

        bool IPointerRoutedEventProperties.RightButton
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.IsRightButtonPressed;
            }
        }
        int IPointerRoutedEventProperties.MouseWheelDelta
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.MouseWheelDelta;
            }
        }
        bool IPointerRoutedEventProperties.IsHorizontalWheel
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.IsHorizontalMouseWheel;
            }
        }
        public PointerRoutedEventArgsWrapper(PointerEvent pointerEvent)
        {
            _pointerEvent = pointerEvent;
        }
    }
}
