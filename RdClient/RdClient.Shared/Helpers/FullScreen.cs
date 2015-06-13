using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class FullScreen : IFullScreen
    {
        public event EventHandler UserInteractionModeChange;
        private void EmitUserInteractionModeChange()
        {
            if(UserInteractionModeChange != null)
            {
                UserInteractionModeChange(this, EventArgs.Empty);
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

        public event EventHandler IsFullScreenModeChange;
        private void EmitIsFullScreenModeChange()
        {
            if(IsFullScreenModeChange != null)
            {
                IsFullScreenModeChange(this, EventArgs.Empty);
            }
        }

        private bool _previousIsFullScreenMode;
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
            _previousIsFullScreenMode = this.IsFullScreenMode;
            _previousUserInteractionMode = this.UserInteractionMode;

            Window.Current.CoreWindow.SizeChanged += OnSizeChanged;
        }
    }
}
