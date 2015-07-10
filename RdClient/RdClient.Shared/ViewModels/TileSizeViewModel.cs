using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Helpers;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.ViewModels
{
    public class TileSizeViewModel
    {
        public Size TileSize
        {
            get
            {
                if(this.WindowSize.Size.Width < GlobalConstants.NarrowLayoutMaxWidth)
                {
                    // phone or narrow layout
                    return new Size(144, 80);
                }
                else if (this.ScreenProperties.Resolution.Width <= 1365.0 && this.ScreenProperties.Resolution.Height <= 1365.0)
                {
                    // medium screens, max dimension <= 1365
                    return new Size(236, 132);
                }
                else
                {
                    // large screens, max dimensions > 1365
                    return new Size(296, 164); // default
                }
            }
        }

        public IWindowSize WindowSize { private get; set; }
        public IScreenProperties ScreenProperties { private get; set; }
    }
}
