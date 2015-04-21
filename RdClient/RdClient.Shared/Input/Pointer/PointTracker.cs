using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointTrackerException : Exception
    {
        public PointTrackerException(string message) : base(message)
        {

        }
    }

    public class PointTracker : IPointTracker
    {
        private Dictionary<UInt32, Point> _trackedPoints = new Dictionary<UInt32, Point>();
        private LinkedList<UInt32> _pointSequence = new LinkedList<UInt32>();
        private Point _lastCenter = new Point(0, 0);
        private Point _lastFirstTouch = new Point(0, 0);

        public void Track(Point point, UInt32 id)
        {
            if(_pointSequence.Contains(id) == false)
            {
                _pointSequence.AddLast(id);
            }

            Point p = new Point() { X = point.X, Y = point.Y };

            _trackedPoints[id] = p;

            if(_pointSequence.Count == 1)
            {
                _lastFirstTouch = p;
            }
        }

        public void Reset()
        {
            _trackedPoints.Clear();
            _pointSequence.Clear();
        }

        public void Untrack(UInt32 id)
        {
            if(_pointSequence.Contains(id))
            {
                _pointSequence.Remove(id);
                _trackedPoints.Remove(id);
            }
        }

        public bool IsTracked(UInt32 id)
        {
            return _trackedPoints.ContainsKey(id);
        }

        public int Contacts
        {
            get
            {
                return _pointSequence.Count;
            }
        }

        public Point Center
        {
            get
            {
                if(_pointSequence.Count == 0)
                {
                    return _lastCenter;
                }
                else if(_pointSequence.Count == 1)
                {
                    return _trackedPoints[_pointSequence.First.Value];
                }
                else
                {
                    _lastCenter = new Point()
                        {
                            X = (_trackedPoints[_pointSequence.ElementAt(0)].X + _trackedPoints[_pointSequence.ElementAt(1)].X) / 2,
                            Y = (_trackedPoints[_pointSequence.ElementAt(0)].Y + _trackedPoints[_pointSequence.ElementAt(1)].Y) / 2
                        };

                    return _lastCenter;
                }
            }
        }

        public Point FirstTouch
        {
            get
            {
                if(_pointSequence.Count == 0)
                {
                    return _lastFirstTouch;
                }
                else
                {
                    return _trackedPoints[_pointSequence.ElementAt(0)];
                }
            }
        }
    }
}
