using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
        Zoom,
        Pan
    }

    public class StateEvent<TInput, TContext>
    {
        public TInput Input { get; set; }
        public TContext Context { get; set; }
    }

    public class TouchContext : IPointerEventConsumer, ITouchContext
    {
        public event System.EventHandler<PointerEvent> ConsumedEvent;

        private Dictionary<uint, PointerEventTrace> _trackedPointerEvents = new Dictionary<uint, PointerEventTrace>();

        private uint _mainPointerId;
        private uint _secondaryPointerId;
        private GestureType _activeGesture;
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

            _activeGesture = GestureType.Idle;
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
                PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];
                result = Math.Abs(eventTrace.DeltaXTo(pointerEvent)) > GlobalConstants.TouchMoveThreshold
                    || Math.Abs(eventTrace.DeltaYTo(pointerEvent)) > GlobalConstants.TouchMoveThreshold;
            }

            return result;
        }

        private DragOrientation DragOrientation(PointerEvent pointerEvent)
        {
            DragOrientation orientation = Mouse.DragOrientation.Unknown;

            if (this.IsPointerDoubleTracked(pointerEvent))
            {
                PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];
                double deltaX = Math.Abs(eventTrace.DeltaXTo(pointerEvent));
                double deltaY = Math.Abs(eventTrace.DeltaYTo(pointerEvent));
                double delta = Math.Pow(deltaX, 2) - Math.Pow(deltaY * deltaY, 2);

                if (delta > GlobalConstants.TouchOrientationDeltaThreshold)
                {
                    orientation = Mouse.DragOrientation.Horizontal;
                }
                else if (delta < -GlobalConstants.TouchOrientationDeltaThreshold)
                {
                    orientation = Mouse.DragOrientation.Vertical;
                }
            }

            return orientation;
        }

        public virtual void MouseScroll(PointerEvent pointerEvent)
        {
            if (this.IsPointerDoubleTracked(pointerEvent))
            {
                DragOrientation orientation = this.DragOrientation(pointerEvent);

                PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];
                double delta = 0.0;

                if(orientation == Mouse.DragOrientation.Vertical)
                {
                    delta = eventTrace.DeltaYTo(pointerEvent);
                    PointerManipulator.SendMouseWheel((int)delta * GlobalConstants.TouchScrollFactor, false);
                }
                else if(orientation == Mouse.DragOrientation.Horizontal)
                {
                    delta = eventTrace.DeltaXTo(pointerEvent);
                    PointerManipulator.SendMouseWheel((int)delta * GlobalConstants.TouchScrollFactor, true);
                }

                _activeGesture = GestureType.Scrolling;
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
            _activeGesture = GestureType.Unknown; // gesture not yet detected
        }

        /// <summary>
        /// Complete 2 fingers gesture
        /// </summary>
        /// <param name="pointerEvent">last pointer event - right/left finger up</param>
        public virtual void CompleteGesture(PointerEvent pointerEvent)
        {
            // should stop tracking positions from both fingers
            _secondaryPointerId = _mainPointerId;
            _activeGesture = GestureType.Idle; // no active gesture
        }


        // 2 fingers scrolling (or panning, if supported)
        public virtual bool IsScrolling(PointerEvent pointerEvent)
        {
            Contract.Assert(2 == this.NumberOfContacts(pointerEvent));
            Contract.Assert(_secondaryPointerId != _mainPointerId);

            bool result = false;

            uint firstPointerId = pointerEvent.PointerId;
            uint secondPointerId = (_mainPointerId == pointerEvent.PointerId) ? _secondaryPointerId : _mainPointerId;

            PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];

            PointerEventTrace firstEventTrace = new PointerEventTrace(pointerEvent, eventTrace.LastEvent);
            PointerEventTrace secondEventTrace = _trackedPointerEvents[secondPointerId];

            if (firstEventTrace.IsUpdated && secondEventTrace.IsUpdated)
            {
                // same deltas (less delta error) means double finger panning or scrolling, depending on context
                if (Math.Abs(firstEventTrace.DeltaX - secondEventTrace.DeltaX) < GlobalConstants.TouchPanDeltaThreshold
                    && Math.Abs(firstEventTrace.DeltaY - secondEventTrace.DeltaY) < GlobalConstants.TouchPanDeltaThreshold)
                {
                    result = true;
                }
            }

            return result;
        }

        public virtual bool IsPanning(PointerEvent pointerEvent)
        {
            // panning is the same as scrolling but starts from zooming.
            if (GestureType.Scrolling != _activeGesture && GestureType.Scrolling != _activeGesture)
            {
                return false;
            }

            return IsScrolling(pointerEvent);
        }

        public virtual bool IsZooming(PointerEvent pointerEvent)
        {
            Contract.Assert(2 == this.NumberOfContacts(pointerEvent));
            Contract.Assert(_secondaryPointerId != _mainPointerId);

            uint firstPointerId = pointerEvent.PointerId;
            uint secondPointerId = (_mainPointerId == pointerEvent.PointerId) ? _secondaryPointerId : _mainPointerId;

            PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];

            PointerEventTrace firstEventTrace = new PointerEventTrace(pointerEvent, eventTrace.LastEvent);
            PointerEventTrace secondEventTrace = _trackedPointerEvents[secondPointerId];


            bool isZoomGesture = false;
            if (firstEventTrace.IsUpdated && secondEventTrace.IsUpdated)
            {
                isZoomGesture = ( Math.Abs(firstEventTrace.DeltaX - secondEventTrace.DeltaX) > GlobalConstants.TouchZoomDeltaThreshold )
                              || ( Math.Abs(firstEventTrace.DeltaY - secondEventTrace.DeltaY) > GlobalConstants.TouchZoomDeltaThreshold ) ;
                
                // opposite deltas means Pinch&Zoom
                if (isZoomGesture && Math.Abs(firstEventTrace.DeltaX - secondEventTrace.DeltaX) > GlobalConstants.TouchZoomDeltaThreshold)
                {
                    // detected movement on x axis
                    if ((firstEventTrace.DeltaX * secondEventTrace.DeltaX) >= 0)
                    {
                        // not opposite
                        isZoomGesture = false;
                    }
                    else if (Math.Abs(firstEventTrace.DeltaX) < GlobalConstants.TouchZoomDeltaThreshold || Math.Abs(secondEventTrace.DeltaX) < GlobalConstants.TouchZoomDeltaThreshold)
                    {
                        // one of the pointers didn't move
                        isZoomGesture = false;
                    }
                }

                if (isZoomGesture && Math.Abs(firstEventTrace.DeltaY - secondEventTrace.DeltaY) > GlobalConstants.TouchZoomDeltaThreshold)
                {
                    // detected movement on y axis
                    if ((firstEventTrace.DeltaY * secondEventTrace.DeltaY) >= 0)
                    {
                        // not opposite
                        isZoomGesture = false;
                    }
                    else if (Math.Abs(firstEventTrace.DeltaY) < GlobalConstants.TouchZoomDeltaThreshold || Math.Abs(secondEventTrace.DeltaY) < GlobalConstants.TouchZoomDeltaThreshold)
                    {
                        // one of the pointers didn't move
                        isZoomGesture = false;
                    }
                }
            }

            return isZoomGesture;
        }


        public virtual void ApplyZoom(PointerEvent pointerEvent)
        {
            uint firstPointerId = pointerEvent.PointerId;
            uint secondPointerId = (_mainPointerId == pointerEvent.PointerId) ? _secondaryPointerId : _mainPointerId;

            PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];

            PointerEventTrace firstEventTrace = new PointerEventTrace(pointerEvent, eventTrace.LastEvent);
            PointerEventTrace secondEventTrace = _trackedPointerEvents[secondPointerId];

            if (firstEventTrace.IsUpdated && secondEventTrace.IsUpdated)
            {
                // Pitagora
                double currentDistance =
                    Math.Sqrt(Math.Pow(secondEventTrace.LastEvent.Position.X - firstEventTrace.LastEvent.Position.X, 2)
                             + Math.Pow(secondEventTrace.LastEvent.Position.Y - firstEventTrace.LastEvent.Position.Y, 2)
                             );
                double prevDistance =
                    Math.Sqrt(Math.Pow(secondEventTrace.PreviousEvent.Position.X - firstEventTrace.PreviousEvent.Position.X, 2)
                             + Math.Pow(secondEventTrace.PreviousEvent.Position.Y - firstEventTrace.PreviousEvent.Position.Y, 2)
                             );

                double centerX = (secondEventTrace.LastEvent.Position.X + firstEventTrace.LastEvent.Position.X) * 0.5;
                double centerY = (secondEventTrace.LastEvent.Position.Y + firstEventTrace.LastEvent.Position.Y) * 0.5;
                PointerManipulator.SendPinchAndZoom(centerX, centerY, prevDistance, currentDistance);

                _activeGesture = GestureType.Zooming;
            }
        }

        public virtual void ApplyPan(PointerEvent pointerEvent)
        {
            PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];
            PointerManipulator.SendPanAction(eventTrace.DeltaX, eventTrace.DeltaY);
            _activeGesture = GestureType.Panning;
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

            if (this.IsPointerTracked(pointerEvent))
            {
                PointerEventTrace eventTrace = _trackedPointerEvents[pointerEvent.PointerId];
                deltaX = eventTrace.DeltaXTo(pointerEvent) * this.PointerManipulator.MouseAcceleration;
                deltaY = eventTrace.DeltaYTo(pointerEvent) * this.PointerManipulator.MouseAcceleration;
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
                    _trackedPointerEvents[pointerEvent.PointerId].SetLastEvent(pointerEvent);
                }
                else
                {
                    _trackedPointerEvents[pointerEvent.PointerId] = new PointerEventTrace(pointerEvent);
                }
                
            }
            else if (pointerEvent.LeftButton == false && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId))
            {
                _trackedPointerEvents.Remove(pointerEvent.PointerId);
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

        /// <summary>
        /// detects if pointerId for the current event is tracked as primary.
        /// </summary>
        /// <param name="pointerEvent">the current poiter event</param>
        /// <returns>true, if tracked as primary pointer</returns>
        private bool IsPointerTracked(PointerEvent pointerEvent)
        {
            return (pointerEvent.PointerId == _mainPointerId && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId));
        }

        /// <summary>
        /// detects if pointerId for the current event is tracked either as primary or secondary.
        /// </summary>
        /// <param name="pointerEvent">the current poiter event</param>
        /// <returns>true, if tracked as primary pointer</returns>
        private bool IsPointerDoubleTracked(PointerEvent pointerEvent)
        {
            return ( ( pointerEvent.PointerId == _mainPointerId || pointerEvent.PointerId == _secondaryPointerId)
                && _trackedPointerEvents.ContainsKey(pointerEvent.PointerId));
        }
    }
}
