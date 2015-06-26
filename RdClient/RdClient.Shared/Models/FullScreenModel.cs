using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;
using Windows.UI.ViewManagement;


namespace RdClient.Shared.Models
{
    public class FullScreenModel : IFullScreenModel
    {
        private IFullScreen _fullScreen;
        private EventHandler _enteringFullScreen, _exitingFullScreen;

        event EventHandler IFullScreenModel.EnteringFullScreen
        {
            add { _enteringFullScreen += value; }
            remove { _enteringFullScreen -= value; }
        }

        event EventHandler IFullScreenModel.ExitingFullScreen
        {
            add { _exitingFullScreen += value; }
            remove { _exitingFullScreen -= value; }
        }

        public IFullScreen FullScreen
        {
            set
            {
                _fullScreen = new FullScreen();
                _fullScreen.UserInteractionModeChanged += (s, o) => EmitUserInteractionModeChange();
                _fullScreen.IsFullScreenModeChanged += (s, o) => EmitFullScreenChange();
            }
        }

        public void EnterFullScreen()
        {
            if (null != _enteringFullScreen)
                _enteringFullScreen(this, EventArgs.Empty);
            _fullScreen.EnterFullScreen();
        }

        public void ExitFullScreen()
        {
            if (null != _exitingFullScreen)
                _exitingFullScreen(this, EventArgs.Empty);
            _fullScreen.ExitFullScreen();
        }

        public event EventHandler FullScreenChange;
        private void EmitFullScreenChange()
        {
            if(FullScreenChange != null)
            {
                FullScreenChange(this, EventArgs.Empty);
            }
        }

        public event EventHandler UserInteractionModeChange;
        private void EmitUserInteractionModeChange()
        {
            if(UserInteractionModeChange != null)
            {
                UserInteractionModeChange(this, EventArgs.Empty);
            }
        }

        public UserInteractionMode UserInteractionMode
        {
            get
            {
                return _fullScreen.UserInteractionMode;
            }
        }

        public bool IsFullScreenMode
        {
            get
            {
                return _fullScreen.IsFullScreenMode;
            }
        }
    }
}
