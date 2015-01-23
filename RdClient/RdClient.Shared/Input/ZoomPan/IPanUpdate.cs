using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public abstract class IPanUpdate
    {
        abstract public double X { get; protected set; }
        abstract public double Y { get; protected set; }
    }
}
