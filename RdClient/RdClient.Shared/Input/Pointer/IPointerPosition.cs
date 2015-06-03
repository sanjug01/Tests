using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    // the implementation of this interface is responsible for 
    // keeping the position of the local cursor in sync between different input modes
    public interface IPointerPosition
    {
        event EventHandler<Point> PositionChanged;
        Point ViewportPosition { get; set; }
        Point SessionPosition { get; }

        void Reset(IRenderingPanel renderingPanel, IExecutionDeferrer executionDeferrer);
    }
}
