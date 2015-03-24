using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public enum PointerMoveOrientation
    {
        Horizontal,
        Vertical
    }

    public class PointerContext : IPointerContext
    {
        private const int _traceSize = 3;
        private Dictionary<uint, ListTrace<PointerEvent>> _pointerTraces = new Dictionary<uint, ListTrace<PointerEvent>>();
        private List<uint> _pointerSequence = new List<uint>();
        private DoubleClickTimer _doubleClickTimer;

        public PointerContext(ITimer timer)
        {
            _doubleClickTimer = new DoubleClickTimer(timer, 300);
        }

        public void Reset()
        {
            Timer.Stop();
            _pointerSequence.Clear();
            _pointerTraces.Clear();
        }

        private IPointerControl _control;
        public IPointerControl Control
        {
            get { return _control; }
            set 
            { 
                _control = value;
                _doubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.LeftClick, _control.MouseLeftClick);
                _doubleClickTimer.AddAction(DoubleClickTimer.ClickTimerType.RightClick, _control.MouseRightClick);
            }
        }

        public DoubleClickTimer Timer
        {
            get { return _doubleClickTimer; }
        }

        public Point LastMoveVector { get; private set; }
        public double LastMoveDistance { get; private set; }
        public PointerMoveOrientation LastMoveOrientation { get; private set; }

        public double LastSpreadDelta { get; private set; }
        public Point LastSpreadCenter { get; private set; }

        public void TrackEvent(PointerEvent pointerEvent)
        {
            if(pointerEvent.ActionType == TouchEventType.Down ||
                (pointerEvent.ActionType == TouchEventType.Update && false == _pointerTraces.ContainsKey(pointerEvent.PointerId)))
            {
                _pointerTraces[pointerEvent.PointerId] = new ListTrace<PointerEvent>(PointerContext._traceSize);
            }

            if(pointerEvent.ActionType == TouchEventType.Down || pointerEvent.ActionType == TouchEventType.Update)
            {
                _pointerTraces[pointerEvent.PointerId].Add(pointerEvent);
                _pointerSequence.Add(pointerEvent.PointerId);
            }

            if((pointerEvent.ActionType == TouchEventType.Up || pointerEvent.ActionType == TouchEventType.Unknown) && _pointerTraces.ContainsKey(pointerEvent.PointerId))
            {
                _pointerTraces.Remove(pointerEvent.PointerId);
                _pointerSequence.Remove(pointerEvent.PointerId);
            }
        }

        public bool SpreadThresholdExceeded(PointerEvent pointerEvent)
        {
            if(_pointerSequence.Count < 2)
            {
                return false;
            }

            if(_pointerSequence[0] != pointerEvent.PointerId && _pointerSequence[1] != pointerEvent.PointerId)
            {
                return false;
            }

            Point oldLeft = _pointerTraces[_pointerSequence[0]].Last().Position;
            Point oldRight = _pointerTraces[_pointerSequence[1]].Last().Position;
            double oldDelta = Distance(oldLeft, oldRight);

            Point newLeft;
            Point newRight;

            if(_pointerSequence[0] == pointerEvent.PointerId)
            {
                newLeft = pointerEvent.Position;
                newRight = _pointerTraces[_pointerSequence[1]].Last().Position;
            }
            else
            {
                newLeft = _pointerTraces[_pointerSequence[0]].Last().Position;
                newRight = pointerEvent.Position;
            }

            double newDelta = Distance(newLeft, newRight);

            double spreadDelta = Math.Abs(newDelta - oldDelta);

            if(spreadDelta > GlobalConstants.TouchZoomDeltaThreshold)
            {
                LastSpreadDelta = spreadDelta;
                LastSpreadCenter = new Point((newRight.X - newLeft.X) / 2, (newRight.Y - newLeft.Y) / 2);

                return true;
            }
            else
            {
                return false;
            }
        }

        private double Distance(Point a, Point b)
        {
            double dX = b.X - a.X;
            double dY = b.Y - a.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            double dX;
            double dY;
            double delta;

            if(false == _pointerTraces.ContainsKey(pointerEvent.PointerId))
            {
                if(pointerEvent.Inertia)
                {
                    dX = pointerEvent.Delta.X;
                    dY = pointerEvent.Delta.Y;
                    delta = Math.Sqrt(dX * dX + dY * dY);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Point current = pointerEvent.Position;
                Point last = _pointerTraces[pointerEvent.PointerId].Last().Position;
                dX = current.X - last.X;
                dY = current.Y - last.Y;
                delta = Math.Sqrt(dX * dX + dY * dY) * GlobalConstants.MouseAcceleration;
            }

            if(delta > GlobalConstants.TouchMoveThreshold || pointerEvent.Inertia)
            {
                LastMoveVector = new Point(dX, dY);
                LastMoveDistance = delta;
                if(dX * dX > dY * dY)
                {
                    LastMoveOrientation = PointerMoveOrientation.Horizontal;
                }
                else
                {
                    LastMoveOrientation = PointerMoveOrientation.Vertical;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public int NumberOfContacts(PointerEvent pointerEvent)
        {
            if(pointerEvent.ActionType == TouchEventType.Up)
            {
                if(_pointerTraces.ContainsKey(pointerEvent.PointerId))
                {
                    return _pointerTraces.Keys.Count - 1;
                }
                else
                {
                    return _pointerTraces.Keys.Count;
                }
            }

            if (_pointerTraces.ContainsKey(pointerEvent.PointerId))
            {
                return _pointerTraces.Keys.Count;
            }
            else
            {
                return _pointerTraces.Keys.Count + 1;
            }
        }
    }
}
