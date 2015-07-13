namespace RdClient.Shared.Models
{
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models.Viewport;
    using RdClient.Shared.Telemetry;
    using RdClient.Shared.ViewModels;
    using System;
    using Windows.Foundation;

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
        private readonly RelayCommand _zoomInCommand;
        private readonly RelayCommand _zoomOutCommand;
        private ITelemetryClient _telemetryClient;
        private IViewport _viewport;
        private Point _viewportCenter;
        private bool _isZoomedIn = false;
        private ConsumptionModeType _consumptionMode;


        public RelayCommand ZoomInCommand { get { return _zoomInCommand; } }

        public RelayCommand ZoomOutCommand { get { return _zoomOutCommand; } }
        public bool IsZoomedIn { get { return _isZoomedIn; } }

        public void SetTelemetryClient(ITelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        private void ZoomInHandler(object parameter)
        {
            if(_viewport != null)
            {
                _viewport.SetZoom(2, new Point(_viewportCenter.X, _viewportCenter.Y));
                _isZoomedIn = true;
                _zoomInCommand.EmitCanExecuteChanged();
                _zoomOutCommand.EmitCanExecuteChanged();

                if (null != _telemetryClient)
                {
                    ITelemetryEvent te = _telemetryClient.MakeEvent("Zoom");
                    te.AddTag("action", "ZoomIn");
                    te.AddTag("source", "ConnectionBar");
                    te.Report();
                }
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

                if(null != _telemetryClient)
                {
                    ITelemetryEvent te = _telemetryClient.MakeEvent("Zoom");
                    te.AddTag("action", "ZoomOut");
                    te.AddTag("source", "ConnectionBar");
                    te.Report();
                }
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
            _viewport.Changed += OnViewportChanged;
            _zoomInCommand.EmitCanExecuteChanged();
            _zoomOutCommand.EmitCanExecuteChanged();
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            if (_viewport.ZoomFactor > 1.0)
            {
                _isZoomedIn = true;
            }
            else
            {
                _isZoomedIn = false;

            }
        }
    }
}
