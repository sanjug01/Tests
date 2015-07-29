using RdClient.Shared.Helpers;
using RdMock;
using System;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.Test.Mock
{
    public class FullScreen : MockBase, IFullScreen
    {
        public bool IsFullScreenMode { get; set; }

        public UserInteractionMode UserInteractionMode { get; set; }

        public event EventHandler IsFullScreenModeChanged;

        public void EmitIsFullScreenModeChanged()
        {
            if(IsFullScreenModeChanged != null)
            {
                IsFullScreenModeChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler UserInteractionModeChanged;
        public void EmitUserInteractionModeChanged()
        {
            if(UserInteractionModeChanged != null)
            {
                UserInteractionModeChanged(this, EventArgs.Empty);
            }
        }

        public void EnterFullScreen()
        {
            Invoke(new object[] { });
        }

        public void ExitFullScreen()
        {
            Invoke(new object[] { });
        }
    }
}
