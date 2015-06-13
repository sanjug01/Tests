using RdClient.Shared.ViewModels;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;


namespace RdClient.Shared.Models
{
    public class FullScreenModel : IFullScreenModel
    {
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

        public void OnSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs e)
        {
            if(_wasFullScreenMode != ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                _wasFullScreenMode = !_wasFullScreenMode;
                _enterFullScreenCommand.EmitCanExecuteChanged();
                _exitFullScreenCommand.EmitCanExecuteChanged();
            }
        }

        public void ToggleFullScreen()
        {
            if(ApplicationView.GetForCurrentView().IsFullScreenMode)
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
                return UIViewSettings.GetForCurrentView().UserInteractionMode;
            }
        }

        public bool IsFullScreenMode
        {
            get
            {
                return ApplicationView.GetForCurrentView().IsFullScreenMode;
            }
        }

        public FullScreenModel()
        {
            _wasFullScreenMode = ApplicationView.GetForCurrentView().IsFullScreenMode;

            Window.Current.CoreWindow.SizeChanged += OnSizeChanged;

            _enterFullScreenCommand = new RelayCommand(
                o =>
                {
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                },
                o => 
                {
                    return ApplicationView.GetForCurrentView().IsFullScreenMode == false;
                });

            _exitFullScreenCommand = new RelayCommand(
                o => ApplicationView.GetForCurrentView().ExitFullScreenMode(),
                o =>
                {
                    return ApplicationView.GetForCurrentView().IsFullScreenMode == true;
                });

        }
    }
}
