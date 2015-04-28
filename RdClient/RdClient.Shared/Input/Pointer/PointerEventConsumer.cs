﻿using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer
{


    public class PointerModeConsumer : IPointerEventConsumer
    {
        private IPointTracker _tracker = new PointTracker();
        public IPointTracker Tracker { get { return _tracker; } }

        private IStateMachine<PointerModeState, PointerStateMachineEvent> _stateMachine;
        PointerStateMachineEvent _stateMachineEvent;
        DoubleClickTimer _timer;

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public PointerModeConsumer(ITimer timer, IPointerControl control)
        {
            _timer = new DoubleClickTimer(timer, 300);

            _stateMachine = new StateMachine<PointerModeState, PointerStateMachineEvent>();
            _stateMachineEvent = new PointerStateMachineEvent() { Input = null, Tracker = _tracker, Timer = _timer, Control = control };

            _timer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, (o) => _stateMachineEvent.Control.LeftClick(o.Position));
            _timer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, (o) => _stateMachineEvent.Control.RightClick(o.Position));

            PointerModeTransitions.AddTransitions(ref _stateMachine);

            Reset();
        }

        public void Reset()
        {
            Tracker.Reset();
            _stateMachine.SetStart(PointerModeState.Idle);
        }

        private void EmitConsumedEvent(IPointerEventBase e)
        {
            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, e);
            }
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

            EmitConsumedEvent(pointerEvent);
        }
    }
}
