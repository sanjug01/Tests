using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public enum DragOrientation
    {
        Horizontal,
        Vertical,
        Unknown
    }

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
        RightDrag,
        Scroll
    }
    public class StateEvent<TInput, TContext>
    {
        public TInput Input { get; set; }
        public TContext Context { get; set; }
    }

    public class TouchContext : IPointerEventConsumer, ITouchContext
    {
        private readonly double PanDeltaThreshold = 2.0; // min for panning
        private readonly double ZoomDeltaThreshold = 5.0; // min for zooming
        private readonly double MoveThreshold = 0.01; 
        private readonly double OrientationDeltaThreshold = 0.01;
        private readonly int ScrollFactor = 5; 

        public event System.EventHandler<PointerEvent> ConsumedEvent;

        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private Dictionary<uint, PointerEvent> _trackedPrevPointerEvents = new Dictionary<uint, PointerEvent>();

        private uint _mainPointerId;
        private uint _secondaryPointerId;
        public DoubleClickTimer DoubleClickTimer { get; private set; }

        private IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> _stateMachine;
        public IPointerManipulator PointerManipulator { get; private set; }

        public TouchContext(ITimer timer, 
                            IPointerManipulator manipulator, 
                            IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine)
        {
            PointerManipulator = manipulator;

            DoubleClickTimer = new DoubleClickTimer(timer, 300);
            DoubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, MouseLeftClick);
            DoubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, MouseRightClick);

            _stateMachine = stateMachine;
        }

        public virtual int NumberOfContacts(PointerEvent pointerEvent)
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

        public virtual bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            bool result = false;

            if (  (pointerEvent.PointerId == _mainPointerId || pointerEvent.PointerId == _secondaryPointerId)
                && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X) > MoveThreshold || Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y) > MoveThreshold;
            }

            return result;
        }

        private DragOrientation DragOrientation(PointerEvent pointerEvent)
        {
            DragOrientation orientation = Mouse.DragOrientation.Unknown;

            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                double deltaX = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X);
                double deltaY = Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y);
                double delta = Math.Pow(deltaX, 2) - Math.Pow(deltaY * deltaY, 2);

                if (delta > OrientationDeltaThreshold)
                {
                    orientation = Mouse.DragOrientation.Horizontal;
                }
                else if (delta < - OrientationDeltaThreshold)
                {
                    orientation = Mouse.DragOrientation.Vertical;
                }
            }

            return orientation;
        }

        public virtual void MouseScroll(PointerEvent pointerEvent)
        {
            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                DragOrientation orientation = this.DragOrientation(pointerEvent);

                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                double delta = 0.0;

                if(orientation == Mouse.DragOrientation.Vertical)
                {
                    delta = - (lastPointerEvent.Position.Y - pointerEvent.Position.Y);
                    PointerManipulator.SendMouseWheel((int)delta * ScrollFactor, false);
                }
                else if(orientation == Mouse.DragOrientation.Horizontal)
                {
                    delta = - (lastPointerEvent.Position.X - pointerEvent.Position.X);
                    PointerManipulator.SendMouseWheel((int)delta * ScrollFactor, true);
                }
            }
        }


        /// <summary>
        /// Begin 2 fingers gesture
        /// </summary>
        /// <param name="pointerEvent">last pointer event - right finger down</param>
        public virtual void BeginGesture(PointerEvent pointerEvent)
        {
            // TODO: should track positions from both fingers
            _secondaryPointerId = pointerEvent.PointerId;
        }

        /// <summary>
        /// Complete 2 fingers gesture
        /// </summary>
        /// <param name="pointerEvent">last pointer event - right/left finger up</param>
        public virtual void EndGesture(PointerEvent pointerEvent)
        {
            // TODO: should stop tracking positions from both fingers
            _secondaryPointerId = 0;
        }


        public virtual void ApplyGesture(PointerEvent pointerEvent)
        {
            PointerEvent lastPrimaryPointerEvent = null; ;
            PointerEvent secondaryPointerEvent = null;
            PointerEvent lastSecondaryPointerEvent = null;

            try
            {
                if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
                {
                    lastPrimaryPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                    secondaryPointerEvent = _trackedPointerEvents[_secondaryPointerId];
                    lastSecondaryPointerEvent = _trackedPrevPointerEvents[_secondaryPointerId];
                }
                else if (pointerEvent.PointerId == _secondaryPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
                {
                    lastPrimaryPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                    secondaryPointerEvent = _trackedPointerEvents[_mainPointerId];
                    lastSecondaryPointerEvent = _trackedPrevPointerEvents[_mainPointerId];
                }
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine("Could not track the second touch for gesture recognizer!");
                return;
            }

            double deltaX = lastPrimaryPointerEvent.Position.X - pointerEvent.Position.X;
            double deltaY = lastPrimaryPointerEvent.Position.Y - pointerEvent.Position.Y;

            double delta2X = lastSecondaryPointerEvent.Position.X - secondaryPointerEvent.Position.X;
            double delta2Y = lastSecondaryPointerEvent.Position.Y - secondaryPointerEvent.Position.Y;

            // Pitagora
            double currentDistance =
                Math.Sqrt(Math.Pow(secondaryPointerEvent.Position.X - pointerEvent.Position.X, 2)
                         + Math.Pow(secondaryPointerEvent.Position.Y - pointerEvent.Position.Y, 2)
                         );
            double prevDistance =
                Math.Sqrt(Math.Pow(lastSecondaryPointerEvent.Position.X - lastPrimaryPointerEvent.Position.X, 2)
                         + Math.Pow(lastSecondaryPointerEvent.Position.Y - lastPrimaryPointerEvent.Position.Y, 2)
                         );

            Debug.WriteLine("Panning/Zooming deltas L:(" + deltaX + "," + deltaY + ") --- R:(" + delta2X + "," + delta2Y + ")");
            Debug.WriteLine("Panning/Zooming sizes " + prevDistance + " --> " + currentDistance + ")");


            if (Math.Abs(deltaX - delta2X) < PanDeltaThreshold && Math.Abs(deltaY - delta2Y) < PanDeltaThreshold)
            {
                // same deltas (less delta error) means double finger panning
                Debug.WriteLine("Panning....");

            }
            else if (Math.Abs(deltaX - delta2X) > ZoomDeltaThreshold || Math.Abs(deltaY - delta2Y) > ZoomDeltaThreshold)
            {              
                bool isZoomGesture = (Math.Abs(currentDistance - prevDistance) > ZoomDeltaThreshold);
             
                // opposite deltas means Pinch&Zoom
                if (Math.Abs(deltaX - delta2X) > ZoomDeltaThreshold && (deltaX * delta2X) > 0)
                {
                    isZoomGesture = false;
                }

                if (Math.Abs(deltaY - delta2Y) > ZoomDeltaThreshold && (deltaY * delta2Y) > 0)
                {
                    isZoomGesture = false;
                }

                if (isZoomGesture)
                {
                    // todo should calculate zoom factor
                    Debug.WriteLine("Zooming ....");
                }
                else
                {
                    Debug.WriteLine("Not Zooming ....");
                }
            }
            
        }

        public virtual void MouseLeftClick(PointerEvent pointerEvent)
        {
            PointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            PointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public virtual void MouseRightClick(PointerEvent pointerEvent)
        {
            PointerManipulator.SendMouseAction(MouseEventType.RightPress);
            PointerManipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public virtual void MouseMove(PointerEvent pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.Move);
        }

        public virtual void UpdateCursorPosition(PointerEvent pointerEvent)
        {
            double deltaX = 0.0;
            double deltaY = 0.0;

            if (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                deltaX = (pointerEvent.Position.X - lastPointerEvent.Position.X) * this.PointerManipulator.MouseAcceleration;
                deltaY = (pointerEvent.Position.Y - lastPointerEvent.Position.Y) * this.PointerManipulator.MouseAcceleration;
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

                if (_trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
                {
                    _trackedPrevPointerEvents[pointerEvent.PointerId] = _trackedPointerEvents[pointerEvent.PointerId];
                }
                _trackedPointerEvents[pointerEvent.PointerId] = pointerEvent;
            }
            else if (pointerEvent.LeftButton == false && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                _trackedPointerEvents.Remove(pointerEvent.PointerId);
                _trackedPrevPointerEvents.Remove(pointerEvent.PointerId);
            }

            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _stateMachine.SetStart(PointerState.Idle);
            DoubleClickTimer.Stop();
            _trackedPointerEvents.Clear();
        }

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }
    }
}
