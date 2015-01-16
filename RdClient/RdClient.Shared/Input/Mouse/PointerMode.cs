using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public class PointerMode : IPointerEventConsumer
    {
        enum PointerState
        {
            Idle,
            LeftDown,
            LeftDoubleDown,
            RightDown,
            RightDoubleDown,
            Move,
            Inertia,
            LeftDrag,
            RightDrag
        }
        
        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private uint _mainPointerId;
        private DoubleClickTimer _doubleClicktimer;
        
        private StateMachine<PointerState, PointerEvent> _stateMachine = new StateMachine<PointerState, PointerEvent>();
        private IPointerManipulator _pointerManipulator;

        public PointerMode(ITimer timer, IPointerManipulator pointerManipulator)
        {
            _pointerManipulator = pointerManipulator;

            _doubleClicktimer = new DoubleClickTimer(timer, 300);
            _doubleClicktimer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, MouseLeftClick);
            _doubleClicktimer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, MouseRightClick);

            _stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDown,
            (o) =>
            {
                return
                    (o).Inertia == false &&
                    NumberOfContacts(o) == 1 &&
                    _doubleClicktimer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == true;
            },
            (o) => { });
            _stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDoubleDown,
            (o) =>
            {
                return
                    NumberOfContacts(o) == 1 &&
                    _doubleClicktimer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == false;
            },
            (o) => { _doubleClicktimer.Stop(); });
            _stateMachine.AddTransition(PointerState.Idle, PointerState.Inertia,
            (o) => { return (o).Inertia == true; },
            (o) => { MouseMove(o); });

            _stateMachine.AddTransition(PointerState.LeftDown, PointerState.Move,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDown,
            (o) =>
            {
                return
                    NumberOfContacts(o) == 2 &&
                    _doubleClicktimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == true;
            },
            (o) => { });
            _stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDoubleDown,
            (o) =>
            {
                return
                    NumberOfContacts(o) == 2 &&
                    _doubleClicktimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == false;
            },
            (o) => { _doubleClicktimer.Stop();});
            _stateMachine.AddTransition(PointerState.LeftDown, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) =>
            {
                if (_doubleClicktimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick))
                    _doubleClicktimer.Reset(DoubleClickTimer.ClickTimerType.LeftClick);
            });

            _stateMachine.AddTransition(PointerState.RightDown, PointerState.LeftDown,
            (o) => { return NumberOfContacts(o) == 1; },
            (o) => { _doubleClicktimer.Reset(DoubleClickTimer.ClickTimerType.RightClick); });
            _stateMachine.AddTransition(PointerState.RightDown, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { _doubleClicktimer.Reset(DoubleClickTimer.ClickTimerType.RightClick); });

            _stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.LeftDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { UpdateCursorPosition(o); _pointerManipulator.SendMouseAction(MouseEventType.LeftPress); });
            _stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { MouseLeftClick(); MouseLeftClick(); });

            _stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.RightDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { UpdateCursorPosition(o); _pointerManipulator.SendMouseAction(MouseEventType.RightPress); });
            _stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { });

            _stateMachine.AddTransition(PointerState.Move, PointerState.Move,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.Move, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { });

            _stateMachine.AddTransition(PointerState.LeftDrag, PointerState.LeftDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.LeftDrag, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { _pointerManipulator.SendMouseAction(MouseEventType.LeftRelease); });

            _stateMachine.AddTransition(PointerState.RightDrag, PointerState.RightDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.RightDrag, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { _pointerManipulator.SendMouseAction(MouseEventType.RightRelease); });

            _stateMachine.AddTransition(PointerState.Inertia, PointerState.Inertia,
            (o) => { return (o).Inertia == true; },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.Inertia, PointerState.Idle,
            (o) => { return (o).Inertia == false; },
            (o) => { });

            _stateMachine.SetStart(PointerState.Idle);
        }

        private int NumberOfContacts(PointerEvent pointerEvent)
        {
            int result = _trackedPointerEvents.Count;

            if (_trackedPointerEvents.ContainsKey(pointerEvent.PointerId) && !pointerEvent.LeftButton)
            {
                result -= 1;
            }
            else if (!_trackedPointerEvents.ContainsKey(pointerEvent.PointerId) && pointerEvent.LeftButton)
            {
                result += 1;
            }

            return result;
        }

        private bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            bool result = false;

            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X) > 0.5 && Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y) > 0.5;
            }

            return result;
        }

        private void MouseLeftClick()
        {
            _pointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            _pointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        private void MouseRightClick()
        {
            _pointerManipulator.SendMouseAction(MouseEventType.RightPress);
            _pointerManipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        private void MouseMove(PointerEvent pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            _pointerManipulator.SendMouseAction(MouseEventType.Move);
        }

        public void UpdateCursorPosition(PointerEvent pointerEvent)
        {
            double deltaX = 0.0;
            double deltaY = 0.0;

            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                deltaX = pointerEvent.Position.X - lastPointerEvent.Position.X;
                deltaY = pointerEvent.Position.Y - lastPointerEvent.Position.Y;
            }
            else if (pointerEvent.Inertia == true)
            {
                deltaX = pointerEvent.Delta.X;
                deltaY = pointerEvent.Delta.Y;
            }

            _pointerManipulator.MousePosition = new Point(_pointerManipulator.MousePosition.X + deltaX, _pointerManipulator.MousePosition.Y + deltaY);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            _stateMachine.Consume(pointerEvent);

            if (pointerEvent.LeftButton)
            {
                if (_trackedPointerEvents.Count == 0)
                {
                    _mainPointerId = pointerEvent.PointerId;
                }

                _trackedPointerEvents[pointerEvent.PointerId] = pointerEvent;
            }
            else if (pointerEvent.LeftButton == false && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                _trackedPointerEvents.Remove(pointerEvent.PointerId);
            }
        }

        public void Reset()
        {
            _stateMachine.SetStart(PointerState.Idle);
            _doubleClicktimer.Stop();
            _trackedPointerEvents.Clear();
        }
    }
}
