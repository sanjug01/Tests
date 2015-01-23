using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public interface IPanUpdate
    {
        double X { get; }
        double Y { get; }
    }
}
