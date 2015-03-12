using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Helpers
{
    public static class GlobalConstants
    {
        public static readonly ulong MaxDoubleTapUS = 300000; // microseconds      
        public static readonly double DesiredDeceleration = 0.002;

        public static readonly double TouchPanDeltaThreshold = 2.0; // min for panning
        public static readonly double TouchZoomDeltaThreshold = 3.0; // min for zooming
        public static readonly double TouchMoveThreshold = 0.01;
        public static readonly double TouchPanMoveThreshold = 50;
        public static readonly double TouchOrientationDeltaThreshold = 0.01;
        public static readonly int TouchScrollFactor = 5;

        public static readonly double PointerPanBorderOffsetX = 200.0;
        public static readonly double PointerPanBorderOffsetY = 200.0;

        public static readonly double PanKnobWidth = 60.0;
        public static readonly double PanKnobHeight = 60.0; 
    }
}
