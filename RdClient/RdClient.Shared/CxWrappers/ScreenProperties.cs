using Windows.Foundation;

namespace RdClient.Shared.CxWrappers
{
    public class ScreenProperties : IScreenProperties
    {
        RdClientCxHelpers.ScreenProperties _screenPropertiesCx;

        public ScreenProperties()
        {
            _screenPropertiesCx = new RdClientCxHelpers.ScreenProperties();
        }

        public Size Resolution
        {
            get
            {
                return _screenPropertiesCx.Resolution;
            }
        }
    }
}
