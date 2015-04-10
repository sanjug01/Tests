using Windows.Foundation;

namespace RdClient.Shared.Input
{
    public interface IPointTracker
    {
        Point Center { get; }
        int Contacts { get; }

        bool IsTracked(uint id);
        void Track(Point point, uint id);
        void Untrack(uint id);
    }
}