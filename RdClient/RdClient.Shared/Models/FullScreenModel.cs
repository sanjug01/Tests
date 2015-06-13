using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System;
using Windows.UI.ViewManagement;


namespace RdClient.Shared.Models
{
    public class FullScreenModel : IFullScreenModel
    {
        private IFullScreen _fullScreen;

        private bool _wasFullScreenMode;

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


        private readonly RelayCommand _enterFullScreenCommand;
        public RelayCommand EnterFullScreenCommand
        {
            get
            {
                return _enterFullScreenCommand;
            }
        }

        public readonly RelayCommand _exitFullScreenCommand;
        public RelayCommand ExitFullScreenCommand
        {
            get
            {
                return _exitFullScreenCommand;
            }
        }

        public void ToggleFullScreen()
        {
            if(_fullScreen.IsFullScreenMode)
            {
                _exitFullScreenCommand.Execute(null);
            }
            else
            {
                _enterFullScreenCommand.Execute(null);
            }

            _enterFullScreenCommand.EmitCanExecuteChanged();
            _exitFullScreenCommand.EmitCanExecuteChanged();
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

        public FullScreenModel()
        {
            _fullScreen = new FullScreen();
            _fullScreen.UserInteractionModeChange += (s, o) => EmitUserInteractionModeChange();
            _fullScreen.IsFullScreenModeChange += (s, o) => EmitFullScreenChange();

            _wasFullScreenMode = ApplicationView.GetForCurrentView().IsFullScreenMode;

            _enterFullScreenCommand = new RelayCommand(
                o =>
                {
                    _fullScreen.EnterFullScreen();
                },
                o => 
                {
                    return _fullScreen.IsFullScreenMode == false;
                });

            _exitFullScreenCommand = new RelayCommand(
                o => _fullScreen.ExitFullScreen(),
                o =>
                {
                    return _fullScreen.IsFullScreenMode == true;
                });
        }
    }
}
