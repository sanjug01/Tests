using Windows.Graphics.Display;

namespace RdClient.Shared.Helpers
{
    public class ScaleFactor : IScaleFactor
    {
        private ArrayRound _rounder;

        public ScaleFactor()
        {
            _rounder = new ArrayRound(new int[] { 100, 140, 180 });
        }

        public int DeviceScaleFactor
        {
            get
            {
                int factor = (int) (DisplayInformation.GetForCurrentView().LogicalDpi * 100) / 96;
                return _rounder.Round(factor);
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
