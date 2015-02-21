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
        Scroll,
        Gesture // two fingers gesture
    }

    public class StateEvent<TInput, TContext>
    {
        public TInput Input { get; set; }
        public TContext Context { get; set; }
    }

    public class TouchContext : IPointerEventConsumer, ITouchContext
    {
        private readonly double PanDeltaThreshold = 2.0; // min for panning
        private readonly double ZoomDeltaThreshold = 3.0; // min for zooming
        private readonly double MoveThreshold = 0.01; 
        private readonly double OrientationDeltaThreshold = 0.01;
        private readonly int ScrollFactor = 5; 

        public event System.EventHandler<PointerEvent> ConsumedEvent;

        private Dictionary<uint, PointerEvent> _trackedPointerEvents = new Dictionary<uint, PointerEvent>();
        private Dictionary<uint, PointerEvent> _trackedPrevPointerEvents = new Dictionary<uint, PointerEvent>();

        private uint _mainPointerId;
        private uint _secondaryPointerId;
        public GestureType ActiveGesture { get; private set; }
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

            ActiveGesture = GestureType.Idle;
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

            if (this.IsPointerTracked(pointerEvent))
            {
                PointerEvent lastPointerEvent = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(lastPointerEvent.Position.X - pointerEvent.Position.X) > MoveThreshold || Math.Abs(lastPointerEvent.Position.Y - pointerEvent.Position.Y) > MoveThreshold;
            }

            return result;
        }

        private DragOrientation DragOrientation(PointerEvent pointerEvent)
        {
            DragOrientation orientation = Mouse.DragOrientation.Unknown;

            if (this.IsPointerTracked(pointerEvent))
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
            if (this.IsPointerTracked(pointerEvent))
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
            // should track positions from both fingers            
            _secondaryPointerId = pointerEvent.PointerId;
            ActiveGesture = GestureType.Unknown; // gesture not yet detected
        }

        /// <summary>
        /// Complete 2 fingers gesture
        /// </summary>
        /// <param name="pointerEvent">last pointer event - right/left finger up</param>
        public virtual void EndGesture(PointerEvent pointerEvent)
        {
            // should stop tracking positions from both fingers
            _secondaryPointerId = 0;
            ActiveGesture = GestureType.Idle; // no active gesture
        }


        public virtual void ApplyGesture(PointerEvent pointerEvent)
        {
            if(0 == _secondaryPointerId)
            {
                return;
            }

            PointerEvent lastPrimaryPointerEvent = null; ;
            PointerEvent secondaryPointerEvent = null;
            PointerEvent lastSecondaryPointerEvent = null;

            uint firstPointerId;
            uint secondPointerId;
            if (pointerEvent.PointerId == _mainPointerId)
            {
                firstPointerId = _mainPointerId;
                secondPointerId = _secondaryPointerId;
            }
            else if (pointerEvent.PointerId == _secondaryPointerId)
            {
                firstPointerId = _secondaryPointerId;
                secondPointerId = _mainPointerId;
            }
            else
            {
                return;
            }

            try
            {
                if (_trackedPointerEvents.ContainsKey(firstPointerId))
                {
                    lastPrimaryPointerEvent = _trackedPointerEvents[firstPointerId];
                }
                else
                {
                    // first button triggered only one event so far
                    lastPrimaryPointerEvent = pointerEvent;
                }

                secondaryPointerEvent = _trackedPointerEvents[secondPointerId];
                if (_trackedPrevPointerEvents.ContainsKey(secondPointerId))
                {
                    lastSecondaryPointerEvent = _trackedPrevPointerEvents[secondPointerId];
                }
                else
                {
                    // second button triggered only one event so far
                    lastSecondaryPointerEvent = secondaryPointerEvent;
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

            Debug.WriteLine("Gesture deltas L:(" + deltaX + "," + deltaY + ") --- R:(" + delta2X + "," + delta2Y + ")");

            // same deltas (less delta error) means double finger panning or scrolling, depending on context
            if (this.ApplyPanOrScroll(pointerEvent, deltaX, deltaY, delta2X, delta2Y))
            {                
                Debug.WriteLine("Scrolling or Panning.... completed");
            }
            else if (this.ApplyPinchAndZoom(
                        pointerEvent, secondaryPointerEvent, 
                        lastPrimaryPointerEvent, lastSecondaryPointerEvent,
                        deltaX, deltaY, delta2X, delta2Y) )
            {
                Debug.WriteLine("Zooming .... completed");
            }
            else
            {
                // the gesture might be incomplete, pending update from the other pointer
                // should preserve cuurent ActiveGEsture in case it is incomplete
                Debug.WriteLine("Unknown or incomplete gesture ....");
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

            if (this.IsPointerPrimaryTracked(pointerEvent))
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
            _trackedPrevPointerEvents.Clear();
        }

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }

        /// <summary>
        /// detects if pointerId for the current event is tracked as primary.
        /// </summary>
        /// <param name="pointerEvent">the current poiter event</param>
        /// <returns>true, if tracked as primary pointer</returns>
        private bool IsPointerPrimaryTracked(PointerEvent pointerEvent)
        {
            return (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId));
        }

        /// <summary>
        /// detects if pointerId for the current event is tracked either as primary or secondary.
        /// </summary>
        /// <param name="pointerEvent">the current poiter event</param>
        /// <returns>true, if tracked as primary pointer</returns>
        private bool IsPointerTracked(PointerEvent pointerEvent)
        {
            return ( ( pointerEvent.PointerId == _mainPointerId || pointerEvent.PointerId == _secondaryPointerId)
                && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId));
        }

        private bool ApplyPanOrScroll(PointerEvent pointerEvent, double deltaX, double deltaY, double delta2X, double delta2Y)
        {
            // same deltas (less delta error) means double finger panning or scrolling, depending on context
            if (Math.Abs(deltaX - delta2X) < PanDeltaThreshold && Math.Abs(deltaY - delta2Y) < PanDeltaThreshold)
            {
                Debug.WriteLine("Scrolling or Panning....");

                if (GestureType.Zooming == ActiveGesture)
                {
                    // moving from pinch&Zoom is panning instead of scrolling
                    PointerManipulator.SendPanAction(deltaX, deltaY);
                    ActiveGesture = GestureType.Scrolling;
                }
                else
                {
                    this.MouseScroll(pointerEvent);
                    ActiveGesture = GestureType.Scrolling;
                }

                return true;
            }
            return false;
        }

        private bool ApplyPinchAndZoom(
            PointerEvent pointerEvent,
            PointerEvent secondaryPointerEvent,
            PointerEvent lastPrimaryPointerEvent,
            PointerEvent lastSecondaryPointerEvent, 
            double deltaX, double deltaY, double delta2X, double delta2Y)
        {
            if (Math.Abs(deltaX - delta2X) > ZoomDeltaThreshold || Math.Abs(deltaY - delta2Y) > ZoomDeltaThreshold)
            {
                // Pitagora
                double currentDistance =
                    Math.Sqrt(Math.Pow(secondaryPointerEvent.Position.X - pointerEvent.Position.X, 2)
                             + Math.Pow(secondaryPointerEvent.Position.Y - pointerEvent.Position.Y, 2)
                             );
                double prevDistance =
                    Math.Sqrt(Math.Pow(lastSecondaryPointerEvent.Position.X - lastPrimaryPointerEvent.Position.X, 2)
                             + Math.Pow(lastSecondaryPointerEvent.Position.Y - lastPrimaryPointerEvent.Position.Y, 2)
                             );
                Debug.WriteLine("Gesture(zoom) sizes " + prevDistance + " --> " + currentDistance + ")");

                bool isZoomGesture = (Math.Abs(currentDistance - prevDistance) > ZoomDeltaThreshold);
             
                // opposite deltas means Pinch&Zoom
                if (isZoomGesture && Math.Abs(deltaX - delta2X) > ZoomDeltaThreshold)
                {
                    // detected movement on x axis
                    if( (deltaX * delta2X) >= 0 )
                    {
                        // not opposite
                        isZoomGesture = false;
                    }
                    else if( Math.Abs(deltaX) < ZoomDeltaThreshold || Math.Abs(delta2X) < ZoomDeltaThreshold)
                    {
                        // one of the pointers didn't move
                        isZoomGesture = false;
                    }               
                }

                if (isZoomGesture && Math.Abs(deltaY - delta2Y) > ZoomDeltaThreshold)
                {
                    // detected movement on y axis
                    if ((deltaY * delta2Y) >= 0)
                    {
                        // not opposite
                        isZoomGesture = false;
                    }
                    else if (Math.Abs(deltaY) < ZoomDeltaThreshold || Math.Abs(delta2Y) < ZoomDeltaThreshold)
                    {
                        // one of the pointers didn't move
                        isZoomGesture = false;
                    }     
                }

                if (isZoomGesture)
                {
                    // should calculate zoom factor, zoom center
                    Debug.WriteLine("Zooming ....");

                    // center = median of L/R position (current)
                    double centerX = (secondaryPointerEvent.Position.X + pointerEvent.Position.X) * 0.5;
                    double centerY = (secondaryPointerEvent.Position.Y + pointerEvent.Position.Y) * 0.5;
                    double fromLength = prevDistance;
                    double toLength = currentDistance;

                    PointerManipulator.SendPinchAndZoom(centerX, centerY, fromLength, toLength);
                    ActiveGesture = GestureType.Zooming;
                }
                else
                {
                    // unsupported gestures - for example rotation
                    // or incomplete - pending another update for the other pointer
                    // should preserve current ActiveGesture in case it is incomplete
                    Debug.WriteLine("Zooming not confirmed....");
                }
            }
            return false;
        }
    }
}
