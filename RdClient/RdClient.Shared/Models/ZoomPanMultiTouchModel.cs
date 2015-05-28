using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.Viewport;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.ViewModels;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Models
{
    public enum PanDirection
    {
        Left,
        Right,
        Up,
        Down,
        Stopped
    }

    public class ZoomPanModel
    {
        private IViewport _viewport;
        private Point _viewportCenter;


        private readonly RelayCommand _zoomInCommand;
        public RelayCommand ZoomInCommand { get { return _zoomInCommand; } }

        private readonly RelayCommand _zoomOutCommand;
        public RelayCommand ZoomOutCommand { get { return _zoomOutCommand; } }

        private bool _isZoomedIn = false;


        private ConsumptionModeType _consumptionMode;

        private void ZoomInHandler(object parameter)
        {
            if(_viewport != null)
            {
                _viewport.SetZoom(2, new Point(_viewportCenter.X, _viewportCenter.Y));
                _isZoomedIn = true;
                _zoomInCommand.EmitCanExecuteChanged();
                _zoomOutCommand.EmitCanExecuteChanged();
            }
        }

        private void ZoomOutHandler(object parameter)
        {
            if(_viewport != null)
            {
                _viewport.SetZoom(1, new Point(_viewportCenter.X, _viewportCenter.Y));
                _isZoomedIn = false;
                _zoomInCommand.EmitCanExecuteChanged();
                _zoomOutCommand.EmitCanExecuteChanged();
            }
        }

        public void OnConsumptionModeChanged(object sender, ConsumptionModeType consumptionMode)
        {
            _consumptionMode = consumptionMode;
            _zoomInCommand.EmitCanExecuteChanged();
            _zoomOutCommand.EmitCanExecuteChanged();
        }


        public ZoomPanModel()
        {
            _zoomInCommand = new RelayCommand(
                o =>
                {
                    ZoomInHandler(o);
                }, 
                o =>
                {
                    return _isZoomedIn == false && _consumptionMode != ConsumptionModeType.Pointer;
                });
            _zoomOutCommand = new RelayCommand(
                o =>
                {
                    ZoomOutHandler(o);
                }, 
                o =>
                {
                    return _isZoomedIn == true && _consumptionMode != ConsumptionModeType.Pointer;
                });
        }


        public void Reset(IViewport viewport)
        {
            _viewport = viewport;
            _isZoomedIn = false;
            _zoomInCommand.EmitCanExecuteChanged();
            _zoomOutCommand.EmitCanExecuteChanged();
        }
    }
}
