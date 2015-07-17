using Windows.Graphics.Display;

namespace RdClient.Shared.Helpers
{
    public class ScaleFactor : IScaleFactor
    {
        public int DeviceScaleFactor
        {
            get
            {
                return (int) (DisplayInformation.GetForCurrentView().LogicalDpi * 100) / 96;
            }
        }

        public int DesktopScaleFactor
        {
            get
            {
                return (int) DisplayInformation.GetForCurrentView().ResolutionScale;
            }
        }

    }
}
