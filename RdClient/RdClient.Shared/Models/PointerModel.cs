using RdClient.Shared.Converters;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using MousePointer = System.Tuple<int, float, float>;


namespace RdClient.Shared.Models
{
    public class DoubleClickTimer
    {
        private ITimer _timer;
        private double _interval;
        private Action _action;

        private bool _expired;
        public bool IsExpired { get { return _expired; } }

        public DoubleClickTimer(ITimer timer, double interval, Action action)
        {
            _action = action;
            _timer = timer;
            _interval = interval;
            _expired = true;
        }

        public void Reset()
        {
            _expired = false;
            _timer.Stop();
            _timer.Start(() => { _expired = true; _action(); }, TimeSpan.FromMilliseconds(_interval), false);
        }

        public void Stop()
        {
            _expired = true;
            _timer.Stop();
        }
    }

    public class PointerModel
    {
        private Point _cursorPosition = new Point(0.0, 0.0);
        public Point CursorPosition { set { _cursorPosition = value; } }

        private Size _windowSize = new Size(0.0, 0.0);
        public Size WindowSize { set { _windowSize = value; } }

        public event EventHandler<MousePointer> MousePointerChanged;

        private StateMachine _stateMachine = new StateMachine();

        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private PointerType _lastPointerType = PointerType.Mouse;
        private DoubleClickTimer _doubleClicktimer;

        public PointerModel(ITimer timer)
        {
            _doubleClicktimer = new DoubleClickTimer(timer, 750, MouseLeftClick);         

            _stateMachine.AddTransition("Idle", "LeftDown", 
            (o) => { return
                (o as PointerEvent).Inertia == false &&
                NumberOfContacts(o as PointerEvent) == 1 &&
                _doubleClicktimer.IsExpired == true; },
            (o) => { });            
            _stateMachine.AddTransition("Idle", "LeftDoubleDown", 
            (o) => { return 
                NumberOfContacts(o as PointerEvent) == 1 &&
                _doubleClicktimer.IsExpired == false; },
            (o) => { _doubleClicktimer.Stop(); });
            _stateMachine.AddTransition("Idle", "Inertia",
            (o) => { return (o as PointerEvent).Inertia == true; },
            (o) => { MouseMove(o as PointerEvent); });

            _stateMachine.AddTransition("LeftDown", "Move", 
            (o) => { return MoveThresholdExceeded(o as PointerEvent); }, 
            (o) => { MouseMove(o as PointerEvent); });
            _stateMachine.AddTransition("LeftDown", "RightDown",
            (o) => { return NumberOfContacts(o as PointerEvent) == 2; },
            (o) => { });
            _stateMachine.AddTransition("LeftDown", "Idle", 
            (o) => { return NumberOfContacts(o as PointerEvent) == 0; },
            (o) => { _doubleClicktimer.Reset(); });

            _stateMachine.AddTransition("RightDown", "LeftDown",
            (o) => { return NumberOfContacts(o as PointerEvent) == 1; },
            (o) => { MouseRightClick(); });
            _stateMachine.AddTransition("RightDown", "Idle",
            (o) => { return NumberOfContacts(o as PointerEvent) == 0; },
            (o) => { MouseRightClick(); });

            _stateMachine.AddTransition("LeftDoubleDown", "LeftDrag", 
            (o) => { return MoveThresholdExceeded(o as PointerEvent); },
            (o) => { UpdateCursorPosition(o as PointerEvent); ChangeMousePointer(0); });
            _stateMachine.AddTransition("LeftDoubleDown", "Idle",
            (o) => { return NumberOfContacts(o as PointerEvent) == 0; },
            (o) => { MouseLeftClick(); MouseLeftClick(); });

            _stateMachine.AddTransition("Move", "Move",
            (o) => { return  MoveThresholdExceeded(o as PointerEvent); }, 
            (o) => { MouseMove(o as PointerEvent); });
            _stateMachine.AddTransition("Move", "Idle", 
            (o) => { return NumberOfContacts(o as PointerEvent) == 0; }, 
            (o) => { });

            _stateMachine.AddTransition("LeftDrag", "LeftDrag", 
            (o) => { return MoveThresholdExceeded(o as PointerEvent); },
            (o) => { MouseMove(o as PointerEvent); });
            _stateMachine.AddTransition("LeftDrag", "Idle", 
            (o) => { return NumberOfContacts(o as PointerEvent) == 0; },
            (o) => { ChangeMousePointer(1); });

            _stateMachine.AddTransition("Inertia", "Inertia", 
            (o) => { return (o as PointerEvent).Inertia == true; }, 
            (o) => { MouseMove(o as PointerEvent); });
            _stateMachine.AddTransition("Inertia", "Idle", 
            (o) => { return (o as PointerEvent).Inertia == false; }, 
            (o) => { });

            _stateMachine.SetStart("Idle");
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

            if (_trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                deltaX = pointerEvent.Position.X - lastPointerEvent.Position.X;
                deltaY = pointerEvent.Position.Y - lastPointerEvent.Position.Y;
            }
            else if(pointerEvent.Inertia == true)
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

            if(_trackedPointerEvents.ContainsKey(pointerEvent.PointerId) && !pointerEvent.LeftButton)
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

            if(_trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X) > 0.5 && Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y) > 0.5;
            }

            return result;
        }

        private void MouseRecognizer(object parameter)
        {
            PointerEvent pointerEvent = parameter as PointerEvent;

            float x = (float) pointerEvent.Position.X;
            float y = (float) pointerEvent.Position.Y;
            int buttonState = 4;

            if (_trackedPointerEvents.ContainsKey(0))
            {
                if (_trackedPointerEvents[0].LeftButton == false && pointerEvent.LeftButton == true)
                {
                    buttonState = 0;
                }
                else if (_trackedPointerEvents[0].LeftButton == true && pointerEvent.LeftButton == false)
                {
                    buttonState = 1;
                }
                else if (_trackedPointerEvents[0].RightButton == false && pointerEvent.RightButton == true)
                {
                    buttonState = 5;
                }
                else if (_trackedPointerEvents[0].RightButton == true && pointerEvent.RightButton == false)
                {
                    buttonState = 6;
                }
            }

            _cursorPosition.X = x;
            _cursorPosition.Y = y;
            ChangeMousePointer(buttonState);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            //if(_lastPointerType != pointerEvent.PointerType)
            //{
            //    _stateMachine.SetStart("Idle");
            //    _trackedPointerEvents.Clear();
            //}
            //_lastPointerType = pointerEvent.PointerType;

            //if(pointerEvent.PointerType == PointerType.Mouse)
            //{
            //    MouseRecognizer(pointerEvent);
            //    _trackedPointerEvents[0] = pointerEvent;
            //}
            //else if(pointerEvent.PointerType == PointerType.Pen)
            //{
            //    MouseRecognizer(pointerEvent);
            //    _trackedPointerEvents[0] = pointerEvent;
            //}
            //else 
                if (pointerEvent.PointerType == PointerType.Touch)
            {
                _stateMachine.DoTransition(pointerEvent);

                if (pointerEvent.LeftButton)
                {
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
