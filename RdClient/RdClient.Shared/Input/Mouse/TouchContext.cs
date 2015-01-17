using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public enum PointerState
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
    public class StateEvent<TInput, TContext>
    {
        public TInput Input { get; set; }
        public TContext Context { get; set; }
    }

    public class TouchContext : IPointerEventConsumer, ITouchContext
    {        
        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private uint _mainPointerId;
        public DoubleClickTimer DoubleClickTimer { get; set; }

        private static IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> _stateMachine;
        public IPointerManipulator PointerManipulator { get; set; }

        public TouchContext(ITimer timer, IPointerManipulator pointerManipulator, IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine)
        {
            PointerManipulator = pointerManipulator;

            DoubleClickTimer = new DoubleClickTimer(timer, 300);
            DoubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, MouseLeftClick);
            DoubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, MouseRightClick);

            _stateMachine = stateMachine;
        }

        public int NumberOfContacts(PointerEvent pointerEvent)
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

        public bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            bool result = false;

            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X) > 0.5 && Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y) > 0.5;
            }

            return result;
        }

        public void MouseLeftClick()
        {
            PointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            PointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public void MouseRightClick()
        {
            PointerManipulator.SendMouseAction(MouseEventType.RightPress);
            PointerManipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public void MouseMove(PointerEvent pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.Move);
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

            PointerManipulator.MousePosition = new Point(PointerManipulator.MousePosition.X + deltaX, PointerManipulator.MousePosition.Y + deltaY);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            _stateMachine.Consume(new StateEvent<PointerEvent, ITouchContext>() { Input = pointerEvent, Context = this });

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
            DoubleClickTimer.Stop();
            _trackedPointerEvents.Clear();
        }
    }
}
