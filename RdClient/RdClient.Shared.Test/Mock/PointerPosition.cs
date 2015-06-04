using RdClient.Shared.Input.Pointer;
using System;
using Windows.Foundation;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using RdMock;
using RdClient.Shared.Navigation.Extensions;

namespace RdClient.Shared.Test.Mock
{
    public class PointerPosition : MockBase, IPointerPosition
    {
        public Point SessionPosition { get; set; }

        public Point ViewportPosition { get; set; }

        public event EventHandler<Point> PositionChanged;
        public void EmitPositionChanged(Point point)
        {
            if(PositionChanged != null)
            {
                PositionChanged(this, point);
            }
        }

        public void Reset(IRenderingPanel renderingPanel, IExecutionDeferrer executionDeferrer)
        {
            Invoke(new object[] { renderingPanel, executionDeferrer });
        }
    }
}
