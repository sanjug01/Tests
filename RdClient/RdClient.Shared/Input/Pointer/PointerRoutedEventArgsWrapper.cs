using RdClient.Shared.Helpers;
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

        private static MemoizeCodeBlock<PointerEvent, PointerDeviceType> _deviceTypeMemo = 
            new MemoizeCodeBlock<PointerEvent, PointerDeviceType>((p) => { return ((PointerRoutedEventArgs)p.Args).GetCurrentPoint(p.Receiver).PointerDevice.PointerDeviceType; });
        public PointerDeviceType DeviceType
        {
            get
            {
                return PointerRoutedEventArgsWrapper._deviceTypeMemo.GetValue(_pointerEvent);
            }
        }

        private static MemoizeCodeBlock<PointerEvent, uint> _pointerIdMemo =
            new MemoizeCodeBlock<PointerEvent, uint>((p) => { return ((PointerRoutedEventArgs)p.Args).GetCurrentPoint(p.Receiver).PointerId; });
        uint IPointerRoutedEventProperties.PointerId
        {
            get
            {
                return PointerRoutedEventArgsWrapper._pointerIdMemo.GetValue(_pointerEvent);
            }
        }

        private static MemoizeCodeBlock<PointerEvent, Point> _positionMemo =
            new MemoizeCodeBlock<PointerEvent, Point>((p) => { return ((PointerRoutedEventArgs)p.Args).GetCurrentPoint(p.Receiver).Position; });
        Point IPointerEventBase.Position
        {
            get
            {
                return PointerRoutedEventArgsWrapper._positionMemo.GetValue(_pointerEvent);
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
