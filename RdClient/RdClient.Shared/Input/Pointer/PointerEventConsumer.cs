using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Input.Pointer
{


    public class PointerModeConsumer
    {
        private IPointTracker _tracker = new PointTracker();
        public IPointTracker Tracker { get { return _tracker; } }

        private IStateMachine<PointerState, StateMachineEvent> _stateMachine;
        StateMachineEvent _stateMachineEvent;
        DoubleClickTimer _timer;

        public PointerModeConsumer(ITimer timer, IPointerControl control)
        {
            _timer = new DoubleClickTimer(timer, 300);

            _stateMachine = new StateMachine<PointerState, StateMachineEvent>();
            _stateMachineEvent = new StateMachineEvent() { Input = null, Tracker = _tracker, Timer = _timer, Control = control };

            _timer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, (o) => _stateMachineEvent.Control.LeftClick(o));
            _timer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, (o) => _stateMachineEvent.Control.RightClick(o));

            PointerModeTransitions.AddTransitions(ref _stateMachine);
        }

        public void Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties pp = (IPointerRoutedEventProperties)pointerEvent;
                switch (pointerEvent.Action)
                {
                    case PointerEventAction.PointerPressed:
                    case PointerEventAction.PointerMoved:
                        Tracker.Track(pp.Position, pp.PointerId);
                        break;
                    case PointerEventAction.PointerReleased:
                    case PointerEventAction.PointerCanceled:
                        Tracker.Untrack(pp.PointerId);
                        break;
                }
            }

            _stateMachineEvent.Input = pointerEvent;
            _stateMachine.Consume(_stateMachineEvent);
        }
    }
}
