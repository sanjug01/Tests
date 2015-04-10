using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Dictionary<uint, ListTrace<PointerEventOld>> _pointerTraces = new Dictionary<uint, ListTrace<PointerEventOld>>();
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
        public Point LastPanDelta { get; private set; }

        public Point LastSpreadCenter { get; private set; }

        public void TrackEvent(PointerEventOld pointerEvent)
        {
            if(pointerEvent.ActionType == TouchEventType.Down ||
                (pointerEvent.ActionType == TouchEventType.Update && false == _pointerTraces.ContainsKey(pointerEvent.PointerId)))
            {
                _pointerTraces[pointerEvent.PointerId] = new ListTrace<PointerEventOld>(PointerContext._traceSize);
            }

            if(pointerEvent.ActionType == TouchEventType.Down || pointerEvent.ActionType == TouchEventType.Update)
            {
                _pointerTraces[pointerEvent.PointerId].Add(pointerEvent);

                if(pointerEvent.ActionType == TouchEventType.Down)
                {
                    _pointerSequence.Add(pointerEvent.PointerId);
                }
            }

            if((pointerEvent.ActionType == TouchEventType.Up || pointerEvent.ActionType == TouchEventType.Unknown) && _pointerTraces.ContainsKey(pointerEvent.PointerId))
            {
                _pointerTraces.Remove(pointerEvent.PointerId);
                _pointerSequence.Remove(pointerEvent.PointerId);
            }
        }

        public bool SpreadThresholdExceeded(PointerEventOld pointerEvent)
        {
            if(_pointerSequence.Count < 2 || (_pointerSequence[0] != pointerEvent.PointerId && _pointerSequence[1] != pointerEvent.PointerId))
            {
                return false;
            }

            double spreadDelta;

            ListTrace<PointerEventOld> leftTrace = _pointerTraces[_pointerSequence[0]];
            ListTrace<PointerEventOld> rightTrace = _pointerTraces[_pointerSequence[1]];

            int leftCount = leftTrace.Count;
            int rightCount = rightTrace.Count;

            Point oldLeft = leftTrace[leftCount - 1].Position;
            Point oldRight = rightTrace[rightCount - 1].Position;
            double oldDelta = Distance(oldLeft, oldRight);

            Point newLeft = (_pointerSequence[0] == pointerEvent.PointerId) ? pointerEvent.Position : leftTrace[leftCount - 1].Position;
            Point newRight = (_pointerSequence[1] == pointerEvent.PointerId) ? pointerEvent.Position : rightTrace[rightCount - 1].Position;
            double newDelta = Distance(newLeft, newRight);

            spreadDelta = newDelta - oldDelta;

            if(Math.Abs(spreadDelta) > GlobalConstants.TouchZoomDeltaThreshold)
            {
                Debug.WriteLine(spreadDelta);

                LastSpreadDelta = spreadDelta;
                Point oldCenter = new Point((oldRight.X + oldLeft.X) / 2, (oldRight.Y + oldLeft.Y) / 2);
                Point newCenter = new Point((newRight.X + newLeft.X) / 2, (newRight.Y + newLeft.Y) / 2);

                LastPanDelta = new Point(newCenter.X - oldCenter.X, newCenter.Y - oldCenter.Y);

                LastSpreadCenter = newCenter;

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

        public bool MoveThresholdExceeded(PointerEventOld pointerEvent, double threshold = GlobalConstants.TouchMoveThreshold)
        {
            double dX;
            double dY;
            double delta;

            // if the finger is not being tracked, the move threshold is not exceeded
            if(_pointerTraces.ContainsKey(pointerEvent.PointerId) == false ||
                _pointerSequence.Count == 0)
            {
                return false;
            }
            // we only check the move threshold of the first finger which was put down
            else if(_pointerSequence[0] != pointerEvent.PointerId)
            {
                return false;
            }
            else
            {
                Point current = pointerEvent.Position;
                Point last = _pointerTraces[pointerEvent.PointerId].Last().Position;
                dX = current.X - last.X;
                dY = current.Y - last.Y;
                delta = Math.Sqrt(dX * dX + dY * dY) * GlobalConstants.MouseAcceleration;
            }

            if (delta > threshold)
            {
                LastSpreadCenter = new Point(0, 0);
                LastSpreadDelta = 0;
                LastMoveVector = new Point(dX, dY);
                LastPanDelta = LastMoveVector;
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

        public int FirstContactHistoryCount()
        {
            if (_pointerSequence.Count == 0)
                return 0;
            else
                return _pointerTraces[_pointerSequence[0]].Count;
        }


        public int NumberOfContacts(PointerEventOld pointerEvent)
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
