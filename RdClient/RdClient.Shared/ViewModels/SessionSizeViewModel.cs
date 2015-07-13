using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
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
                return this.WindowSize.Size;
            }
        }

        public IWindowSize WindowSize { private get; set; }
    }
}
