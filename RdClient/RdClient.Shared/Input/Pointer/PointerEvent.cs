using System;
using System.Diagnostics;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Input
{


    public class PointerRoutedEventArgsCopy : IPointerRoutedEventProperties
    {
        public PointerEventAction Action { get; private set; }    
        public PointerDeviceType DeviceType { get; private set; }
        public uint PointerId { get; private set; }
        public Point Position { get; private set; }
        public ulong Timestamp { get; private set; }
        public bool LeftButton { get; private set; }
        public bool RightButton { get; private set; }
        public int MouseWheelDelta { get; private set; }
        public bool IsHorizontalWheel { get; private set; }

        public PointerRoutedEventArgsCopy(IPointerRoutedEventProperties wrapper)
        {
            Action = wrapper.Action;
            DeviceType = wrapper.DeviceType;
            PointerId = wrapper.PointerId;
            Position = wrapper.Position;
            Timestamp = wrapper.Timestamp;
            LeftButton = wrapper.LeftButton;
            RightButton = wrapper.RightButton;
            MouseWheelDelta = wrapper.MouseWheelDelta;
            IsHorizontalWheel = wrapper.IsHorizontalWheel;
        }
    }

    public class PointerRoutedEventArgsWrapper : IPointerRoutedEventProperties
    {
        private PointerEvent _pointerEvent;

        public PointerEventAction Action { get { return _pointerEvent.Action; } }

        public PointerDeviceType DeviceType
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.PointerDevice.PointerDeviceType;
            }
        }

        public uint PointerId
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.PointerId;
            }
        }

        public Point Position
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Position;
            }
        }

        public ulong Timestamp
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Timestamp;
            }
        }

        public bool LeftButton
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.IsLeftButtonPressed;
            }
        }

        public bool RightButton
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.IsRightButtonPressed;
            }
        }
        public int MouseWheelDelta
        {
            get
            {
                PointerPoint pp = ((PointerRoutedEventArgs)_pointerEvent.Args).GetCurrentPoint(_pointerEvent.Receiver);
                return pp.Properties.MouseWheelDelta;
            }
        }
        public bool IsHorizontalWheel
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

    public class ManipulationRoutedEventArgsWrapper : IManipulationRoutedEventProperties
    {
        private PointerEvent _pointerEvent;

        public PointerEventAction Action
        {
            get
            {
                return _pointerEvent.Action;
            }
        }

        public ManipulationDelta Cummulative
        {
            get
            {
                switch(_pointerEvent.Type)
                {
                    case PointerEventType.ManipulationCompletedRoutedEventArgs:
                        return ((ManipulationCompletedRoutedEventArgs)_pointerEvent.Args).Cumulative;
                    case PointerEventType.ManipulationDeltaRoutedEventArgs:
                        return ((ManipulationDeltaRoutedEventArgs)_pointerEvent.Args).Cumulative;
                    case PointerEventType.ManipulationInertiaStartingRoutedEventArgs:
                        return ((ManipulationInertiaStartingRoutedEventArgs)_pointerEvent.Args).Cumulative;
                    case PointerEventType.ManipulationStartedRoutedEventArgs:
                        return ((ManipulationStartedRoutedEventArgs)_pointerEvent.Args).Cumulative;
                    case PointerEventType.ManipulationStartingRoutedEventArgs:
                    default:
                        return new ManipulationDelta();
                }
            }
        }

        public ManipulationDelta Delta
        {
            get
            {
                switch (_pointerEvent.Type)
                {
                    case PointerEventType.ManipulationDeltaRoutedEventArgs:
                        return ((ManipulationDeltaRoutedEventArgs)_pointerEvent.Args).Delta;
                    case PointerEventType.ManipulationInertiaStartingRoutedEventArgs:
                        return ((ManipulationInertiaStartingRoutedEventArgs)_pointerEvent.Args).Delta;
                    case PointerEventType.ManipulationStartedRoutedEventArgs:
                        return ((ManipulationStartedRoutedEventArgs)_pointerEvent.Args).Cumulative;
                    case PointerEventType.ManipulationCompletedRoutedEventArgs:
                    case PointerEventType.ManipulationStartingRoutedEventArgs:
                    default:
                        return new ManipulationDelta();
                }
            }
        }
        public Point Position
        {
            get
            {
                switch (_pointerEvent.Type)
                {
                    case PointerEventType.ManipulationDeltaRoutedEventArgs:
                        return ((ManipulationDeltaRoutedEventArgs)_pointerEvent.Args).Position;
                    case PointerEventType.ManipulationStartedRoutedEventArgs:
                        return ((ManipulationStartedRoutedEventArgs)_pointerEvent.Args).Position;
                    case PointerEventType.ManipulationCompletedRoutedEventArgs:
                        return ((ManipulationCompletedRoutedEventArgs)_pointerEvent.Args).Position;
                    case PointerEventType.ManipulationStartingRoutedEventArgs:
                    case PointerEventType.ManipulationInertiaStartingRoutedEventArgs:
                    default:
                        return new Point(double.NaN, double.NaN);
                }
            }
        }

        public bool IsInertial
        {
            get
            {
                switch (_pointerEvent.Type)
                {
                    case PointerEventType.ManipulationDeltaRoutedEventArgs:
                        return ((ManipulationDeltaRoutedEventArgs)_pointerEvent.Args).IsInertial;
                    case PointerEventType.ManipulationInertiaStartingRoutedEventArgs:
                        return true;
                    case PointerEventType.ManipulationCompletedRoutedEventArgs:
                        return ((ManipulationCompletedRoutedEventArgs)_pointerEvent.Args).IsInertial;
                    case PointerEventType.ManipulationStartedRoutedEventArgs:
                    case PointerEventType.ManipulationStartingRoutedEventArgs:
                    default:
                        return false;
                }
            }
        }

        public ManipulationRoutedEventArgsWrapper(PointerEvent pointerEvent)
        {
            _pointerEvent = pointerEvent;
        }
    }

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
                if(_pointerEvent.Action == PointerEventAction.Tapped)
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
                switch(_pointerEvent.Type)
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

    public class PointerEvent
    {
        public PointerEventAction Action { get; private set; }
        public PointerEventType Type { get; private set; }
        public object Args { get; private set; }
        public UIElement Receiver { get; private set; }

        public PointerEvent(PointerEventAction action, PointerEventType type, object args, UIElement receiver)
        {
            Action = action;
            Type = type;
            Args = args;
            Receiver = receiver;
        }
    }
}
