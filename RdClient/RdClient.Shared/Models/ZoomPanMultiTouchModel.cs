using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
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
        private IPointerPosition _pointerPosition;
        private IExecutionDeferrer _deferrer;

        private readonly RelayCommand _zoomInCommand;
        public RelayCommand ZoomInCommand { get { return _zoomInCommand; } }

        private readonly RelayCommand _zoomOutCommand;
        public RelayCommand ZoomOutCommand { get { return _zoomOutCommand; } }

        private bool _isZoomedIn = false;

        private ITimer _timer;

        private double _xThreshold;
        private double _yThreshold;
        private double _panStep;
        private int _timerStep;

        private bool _panning = false;

        private ConsumptionModeType _consumptionMode;

        private void ZoomInHandler(object parameter)
        {
            if(_viewport != null)
            {
                _viewport.Set(2, new Size(_viewportCenter.X, _viewportCenter.Y));
                _isZoomedIn = true;
                _zoomInCommand.EmitCanExecuteChanged();
                _zoomOutCommand.EmitCanExecuteChanged();
            }
        }

        private void ZoomOutHandler(object parameter)
        {
            if(_viewport != null)
            {
                _viewport.Set(1, new Size(_viewportCenter.X, _viewportCenter.Y));
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

        private void PanALittle(PanDirection panDirection)
        {
            switch(panDirection)
            {
                case PanDirection.Up:
                    _deferrer.TryDeferToUI(() => _viewport.PanAndZoom(_viewportCenter, 0, _panStep, 1.0));
                    break;
                case PanDirection.Down:
                    _deferrer.TryDeferToUI(() => _viewport.PanAndZoom(_viewportCenter, 0, -_panStep, 1.0));
                    break;
                case PanDirection.Left:
                    _deferrer.TryDeferToUI(() => _viewport.PanAndZoom(_viewportCenter, _panStep, 0, 1.0));
                    break;
                case PanDirection.Right:
                    _deferrer.TryDeferToUI(() => _viewport.PanAndZoom(_viewportCenter, -_panStep, 0, 1.0));
                    break;
            }
        }

        private PanDirection ShouldPan(Point position)
        {
            if (position.X < _xThreshold)
            {
                return PanDirection.Left;
            }
            else if (position.X > _viewport.Size.Width - _xThreshold)
            {
                return PanDirection.Right;
            }
            else if (position.Y < _yThreshold)
            {
                return PanDirection.Up;
            }
            else if (position.Y > _viewport.Size.Height - _yThreshold)
            {
                return PanDirection.Down;
            }

            return PanDirection.Stopped;
        }

        private void OnPointerPositionChanged(object sender, Point position)
        {
            if(_consumptionMode == ConsumptionModeType.Pointer)
            {
                PanDirection direction = ShouldPan(position);

                if (direction != PanDirection.Stopped)
                {
                    if (_panning == false)
                    {
                        _timer.Start(
                            () => PanALittle(direction),
                            TimeSpan.FromMilliseconds(_timerStep),
                            true);
                        _panning = true;
                    }
                }
                else
                {
                    if (_panning == true)
                    {
                        _timer.Stop();
                        _panning = false;
                    }
                }
            }
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


        public void Reset(IViewport viewport, IPointerPosition pointerPosition, ITimer timer, IExecutionDeferrer deferrer)
        {
            _viewport = viewport;
            _pointerPosition = pointerPosition;
            _timer = timer;
            _deferrer = deferrer;

            _viewportCenter = new Point(_viewport.Size.Width / 2.0, _viewport.Size.Height / 2.0);
            _xThreshold = _viewport.Size.Width * 0.08;
            _yThreshold = _viewport.Size.Height * 0.08;
            _panStep = RdMath.Distance(new Point(_viewport.Size.Width, _viewport.Size.Height)) * 0.1;

            _pointerPosition.PositionChanged += OnPointerPositionChanged;

            _timerStep = 100;

            _panning = false;
        }
    }
}
