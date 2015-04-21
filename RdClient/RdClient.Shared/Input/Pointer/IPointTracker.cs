using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    // the point tracker is used when some intermediate points are required
    // to calculate conditions for input state-machines
    // the uint id is typically the PointerId which comes with the platform PointerEvents
    public interface IPointTracker
    {
        Point FirstTouch { get; }
        Point Center { get; }
        int Contacts { get; }

        bool IsTracked(uint id);
        void Track(Point point, uint id);
        void Untrack(uint id);
        void Reset();
    }
}