using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class FullScreen : IFullScreen
    {
        public event EventHandler UserInteractionModeChanged;
        private void EmitUserInteractionModeChange()
        {
            if(UserInteractionModeChanged != null)
            {
                UserInteractionModeChanged(this, EventArgs.Empty);
            }
        }

        private UserInteractionMode _previousUserInteractionMode;
        public UserInteractionMode UserInteractionMode
        {
            get
            {
                return UIViewSettings.GetForCurrentView().UserInteractionMode;
            }
        }

        public event EventHandler IsFullScreenModeChanged;
        private void EmitIsFullScreenModeChange()
        {
            if(IsFullScreenModeChanged != null)
            {
                IsFullScreenModeChanged(this, EventArgs.Empty);
            }
        }

        private bool _previousIsFullScreenMode;
        private IWindowSize _windowSize;

        public bool IsFullScreenMode
        {
            get
            {
                return ApplicationView.GetForCurrentView().IsFullScreenMode;
            }
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if(_previousIsFullScreenMode != this.IsFullScreenMode)
            {
                _previousIsFullScreenMode = this.IsFullScreenMode;
                EmitIsFullScreenModeChange();
            }

            if(_previousUserInteractionMode != this.UserInteractionMode)
            {
                _previousUserInteractionMode = this.UserInteractionMode;
                EmitUserInteractionModeChange();
            }
        }

        public void EnterFullScreen()
        {
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        public void ExitFullScreen()
        {
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        public FullScreen()
        {
            _windowSize = new WindowSize();
            _previousIsFullScreenMode = this.IsFullScreenMode;
            _previousUserInteractionMode = this.UserInteractionMode;

            _windowSize.SizeChanged += OnSizeChanged;
        }
    }
}
