using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using MousePointer = System.Tuple<int, float, float>;


namespace RdClient.Shared.Input.Mouse
{
    public class DoubleClickTimer
    {
        public enum ClickTimerType
        {
            LeftClick,
            RightClick
        }

        private ITimer _timer;
        private double _interval;
        private Dictionary<ClickTimerType, Action> _actions;
        private ClickTimerType _timerType;

        private bool _expired;
        public bool IsExpired(ClickTimerType timerType)
        {
            bool matchingType = (timerType == _timerType);

            if (matchingType == false)
            {
                return true;
            }
            else
            {
                return _expired;
            }
        }

        public DoubleClickTimer(ITimer timer, double interval)
        {
            _actions = new Dictionary<ClickTimerType, Action>();
            _timer = timer;
            _interval = interval;
            _expired = true;
        }

        public void AddAction(ClickTimerType timerType, Action action)
        {
            _actions[timerType] = action;
        }

        public void Reset(ClickTimerType timerType)
        {
            _timerType = timerType;
            _expired = false;
            _timer.Stop();
            _timer.Start(() => { _expired = true; _actions[timerType](); }, TimeSpan.FromMilliseconds(_interval), false);
        }

        public void Stop()
        {
            _expired = true;
            _timer.Stop();
        }
    }

    public class PointerModel
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

        private Point _cursorPosition = new Point(0.0, 0.0);
        public Point CursorPosition { set { _cursorPosition = value; } }

        private Size _windowSize = new Size(0.0, 0.0);
        public Size WindowSize { set { _windowSize = value; } }

        public event EventHandler<MousePointer> MousePointerChanged;

        private StateMachine<PointerState, PointerEvent> _stateMachine = new StateMachine<PointerState, PointerEvent>();

        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private uint _mainPointerId;
        private PointerType _lastPointerType = PointerType.Mouse;
        private DoubleClickTimer _doubleClicktimer;

        public PointerModel(ITimer timer)
        {
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
            (o) => { UpdateCursorPosition(o); ChangeMousePointer(0); });
            _stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { MouseLeftClick(); MouseLeftClick(); });

            _stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.RightDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { UpdateCursorPosition(o); ChangeMousePointer(5); });
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
            (o) => { ChangeMousePointer(1); });

            _stateMachine.AddTransition(PointerState.RightDrag, PointerState.RightDrag,
            (o) => { return MoveThresholdExceeded(o); },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.RightDrag, PointerState.Idle,
            (o) => { return NumberOfContacts(o) == 0; },
            (o) => { ChangeMousePointer(6); });

            _stateMachine.AddTransition(PointerState.Inertia, PointerState.Inertia,
            (o) => { return (o).Inertia == true; },
            (o) => { MouseMove(o); });
            _stateMachine.AddTransition(PointerState.Inertia, PointerState.Idle,
            (o) => { return (o).Inertia == false; },
            (o) => { });

            _stateMachine.SetStart(PointerState.Idle);
        }

        private void MouseMove(PointerEvent pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            ChangeMousePointer(4);
        }

        private void MouseLeftClick()
        {
            ChangeMousePointer(0);
            ChangeMousePointer(1);
        }

        private void MouseRightClick()
        {
            ChangeMousePointer(5);
            ChangeMousePointer(6);
        }

        private void ChangeMousePointer(int eventType)
        {
            MousePointerChanged(this, new MousePointer(eventType, (float)_cursorPosition.X, (float)_cursorPosition.Y));
        }

        private void UpdateCursorPosition(PointerEvent pointerEvent)
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

            _cursorPosition.X = Math.Min(Math.Max(0.0, _cursorPosition.X + deltaX), _windowSize.Width);
            _cursorPosition.Y = Math.Min(Math.Max(0.0, _cursorPosition.Y + deltaY), _windowSize.Height);
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

        private bool MouseLeftButton(uint id)
        {
            if (_trackedPointerEvents.ContainsKey(id))
            {
                return _trackedPointerEvents[id].LeftButton;
            }
            else
            {
                return false;
            }
        }

        private bool MouseRightButton(uint id)
        {
            if (_trackedPointerEvents.ContainsKey(id))
            {
                return _trackedPointerEvents[id].RightButton;
            }
            else
            {
                return false;
            }
        }

        private void MouseRecognizer(object parameter)
        {
            PointerEvent pointerEvent = parameter as PointerEvent;

            float x = (float)pointerEvent.Position.X;
            float y = (float)pointerEvent.Position.Y;
            int buttonState = 4;

            if (MouseLeftButton(0) == false && pointerEvent.LeftButton == true)
            {
                buttonState = 0;
            }
            else if (MouseLeftButton(0) == true && pointerEvent.LeftButton == false)
            {
                buttonState = 1;
            }
            else if (MouseRightButton(0) == false && pointerEvent.RightButton == true)
            {
                buttonState = 5;
            }
            else if (MouseRightButton(0) == true && pointerEvent.RightButton == false)
            {
                buttonState = 6;
            }
            
            _cursorPosition.X = x;
            _cursorPosition.Y = y;
            ChangeMousePointer(buttonState);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (_lastPointerType != pointerEvent.PointerType)
            {
                _stateMachine.SetStart(PointerState.Idle);
                _doubleClicktimer.Stop();
                _trackedPointerEvents.Clear();
            }
            _lastPointerType = pointerEvent.PointerType;

            if (pointerEvent.PointerType == PointerType.Mouse)
            {
                MouseRecognizer(pointerEvent);
                _trackedPointerEvents[0] = pointerEvent;
            }
            else if (pointerEvent.PointerType == PointerType.Pen)
            {
                MouseRecognizer(pointerEvent);
                _trackedPointerEvents[0] = pointerEvent;
            }
            else if (pointerEvent.PointerType == PointerType.Touch)
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

        }
    }
}
