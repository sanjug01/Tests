using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Helpers
{
    public static class GlobalConstants
    {
        public const ulong MaxDoubleTapUS = 300000; // microseconds      
        public const double DesiredDeceleration = 0.002;
        public const double MouseAcceleration = 1.4;

        public const double TouchPanDeltaThreshold = 2.0; // min for panning
        public const double TouchZoomDeltaThreshold = 3; // min for zooming
        public const double TouchMoveThreshold = 0.01;
        public const double TouchPanMoveThreshold = 50;
        public const double TouchOrientationDeltaThreshold = 0.01;
        public const int TouchScrollFactor = 5;

        public const double PointerPanBorderOffsetX = 200.0;
        public const double PointerPanBorderOffsetY = 200.0;

        public const double PanKnobWidth = 60.0;
        public const double PanKnobHeight = 60.0;

        public const string HelpDocumentUrl = @"http://www.bing.com/search?q=remote+desktop";
        public const string EulaUri = "ms-appx:///Strings/EULA.rtf";
        public const string ThirdPartyUri = "ms-appx:///Strings/ThirdPartyNotices.rtf";
        // we don't have Privacy/Help documents yet
        public const string PrivacyDocUri = "";      // TODO: doc not yet available, using link until then
    }
}
