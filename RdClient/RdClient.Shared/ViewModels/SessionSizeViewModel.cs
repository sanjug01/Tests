using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.ViewModels
{
    public class SessionSizeViewModel
    {
        public Size Size
        {
            get
            {
                if(this.FullScreenModel.UserInteractionMode == UserInteractionMode.Mouse)
                {
                    return this.ScreenProperties.Resolution;
                }
                else
                {
                    return this.WindowSize.Size;
                }
            }
        }

        public IFullScreenModel FullScreenModel { private get; set; }
        public IWindowSize WindowSize { private get; set; }
        public IScreenProperties ScreenProperties { private get; set; }
    }
}
