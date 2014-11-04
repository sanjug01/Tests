using RdClient.Shared;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace RdClient.Helpers
{
    class PhysicalScreenSize : IPhysicalScreenSize
    {
        public double InchesToMillimeters(double inches)
        {
            return inches * 25.4;
        }

        //
        // Returns the physical width and height of the screen (relative to its current orientation)
        // in inches.
        //
        Size InternalGetScreenSize()
        {
            Size physicalSize = new Size();
            DisplayInformation dispInfo = DisplayInformation.GetForCurrentView();
            CoreWindow coreWindow = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            bool fNativeLandscape;
            bool fCurrentLandscape;

            fNativeLandscape = (dispInfo.NativeOrientation == DisplayOrientations.Landscape) || (dispInfo.NativeOrientation == DisplayOrientations.LandscapeFlipped);
            fCurrentLandscape = (dispInfo.CurrentOrientation == DisplayOrientations.Landscape) || (dispInfo.CurrentOrientation == DisplayOrientations.LandscapeFlipped);

            if ((fNativeLandscape && fCurrentLandscape) || (!fNativeLandscape && !fCurrentLandscape))
            {
                //
                // Current orientation (landscape or portrait, ignore flipped) matches the native orientation. 
                //
                physicalSize.Width = coreWindow.Bounds.Width / dispInfo.LogicalDpi;
                physicalSize.Height = coreWindow.Bounds.Height / dispInfo.LogicalDpi;
            }
            else
            {
                //
                // Current orientation does not match the native orientation, so we need to swap RawDpiX 
                // and RawDpiY to account for a 90 degree rotation. 
                //
                physicalSize.Width = coreWindow.Bounds.Width / dispInfo.LogicalDpi;
                physicalSize.Height = coreWindow.Bounds.Height / dispInfo.LogicalDpi;
            }

            return physicalSize;
        }

        public ScreenSize GetScreenSize()
        {
            Size size = InternalGetScreenSize();
            int width = (int) InchesToMillimeters(size.Width);
            int height = (int) InchesToMillimeters(size.Height);

            return new ScreenSize() { Width = width, Height = height };
        }
    }
}