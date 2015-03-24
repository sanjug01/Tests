using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeContext : IPointerModeContext
    {
        private const int _traceSize = 3;
        private Dictionary<uint, ListTrace<PointerEvent>> _pointerTraces = new Dictionary<uint, ListTrace<PointerEvent>>();

        private List<uint> _pointerSequence = new List<uint>();

        private DoubleClickTimer _doubleClickTimer;

        public PointerModeContext(ITimer timer)
        {
            _doubleClickTimer = new DoubleClickTimer(timer, 300);
        }

        public IPointerModeControl Control
        {
            get { throw new NotImplementedException(); }
        }

        public DoubleClickTimer Timer
        {
            get { return _doubleClickTimer; }
        }

        public void TrackEvent(PointerEvent pointerEvent)
        {
            if(pointerEvent.ActionType == TouchEventType.Down ||
                (pointerEvent.ActionType == TouchEventType.Update && false == _pointerTraces.ContainsKey(pointerEvent.PointerId)))
            {
                _pointerTraces[pointerEvent.PointerId] = new ListTrace<PointerEvent>(PointerModeContext._traceSize);
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

            PointerEvent oldLeft = _pointerTraces[_pointerSequence[0]].Last();
            PointerEvent oldRight = _pointerTraces[_pointerSequence[1]].Last();
            double oldDelta = Distance(oldLeft.Position, oldRight.Position);

            PointerEvent newLeft;
            PointerEvent newRight;

            if(_pointerSequence[0] == pointerEvent.PointerId)
            {
                newLeft = pointerEvent;
                newRight = _pointerTraces[_pointerSequence[1]].Last();
            }
            else
            {
                newLeft = _pointerTraces[_pointerSequence[0]].Last();
                newRight = pointerEvent;
            }

            double newDelta = Distance(newLeft.Position, newRight.Position);

            return Math.Abs(newDelta - oldDelta) > GlobalConstants.TouchZoomDeltaThreshold;
        }

        private double Distance(Point a, Point b)
        {
            double dX = b.X - a.X;
            double dY = b.Y - a.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            if(false == _pointerTraces.ContainsKey(pointerEvent.PointerId))
            {
                return false;
            }

            PointerEvent last = _pointerTraces[pointerEvent.PointerId].Last();
            double delta = Distance(last.Position, pointerEvent.Position);

            return delta > GlobalConstants.TouchMoveThreshold;
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
