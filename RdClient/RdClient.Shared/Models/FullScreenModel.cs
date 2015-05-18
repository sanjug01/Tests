using RdClient.Shared.ViewModels;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace RdClient.Shared.Models
{
    public class FullScreenModel
    {
        private bool _wasFullScreenMode;

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
