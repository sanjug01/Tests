using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Input.Pointer
{
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
                switch (_pointerEvent.Type)
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
}
