using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace RdClient.Shared.Input
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

        public void Track(Point point, UInt32 id)
        {
            if(_pointSequence.Contains(id) == false)
            {
                _pointSequence.AddLast(id);
            }

            _trackedPoints[id] = new Point() { X = point.X, Y = point.Y };
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
                    throw new PointTrackerException("no contacts tracked");
                }
                else if(_pointSequence.Count == 1)
                {
                    return _trackedPoints[_pointSequence.First.Value];
                }
                else
                {
                    return new Point()
                    {
                        X = (_trackedPoints[_pointSequence.ElementAt(0)].X + _trackedPoints[_pointSequence.ElementAt(1)].X) / 2,
                        Y = (_trackedPoints[_pointSequence.ElementAt(0)].Y + _trackedPoints[_pointSequence.ElementAt(1)].Y) / 2
                    };
                }
            }
        }
    }
}
