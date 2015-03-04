using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.Input.Mouse;
    using RdClient.Shared.Navigation.Extensions;

    public class PanKnobTransform : IPanKnobTransform 
    {
        public PanKnobTransformType TransformType { get; private set; }

        public PanKnobTransform(PanKnobTransformType type)
        {
            TransformType = type;
        }
    }

    public class PanKnobMoveTransform : IPanKnobTransform
    {
        public PanKnobMoveTransform(double deltaX, double deltaY)
        {
            TransformType = PanKnobTransformType.MoveKnob;
            DeltaX = deltaX;
            DeltaY = deltaX;
        }
        public PanKnobTransformType TransformType { get; private set; }
        public double DeltaX { get; private set; }
        public double DeltaY { get; private set; }
    }

    public class PanKnobPointerEventDispatcher : IPointerEventConsumer
    {

        public event EventHandler<PointerEvent> ConsumedEvent;

        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set { _consumptionMode = value; }
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
        }
    }


    public sealed class PanKnobViewModel : MutableObject
    {
        private IPanKnobTransform _panKnobTransform;
        private PanKnobState _state;
        private bool _isPanning;

        private double _translateXFrom;
        private double _translateXTo;
        private double _translateYFrom;
        private double _translateYTo;
        private ulong _lastTouchTimeStamp;

        private double _panControlOpacity;
        private double _panOrbOpacity;

        private bool _isInertiaNotProcessed;
        private bool _isInertiaEnabled;

        private Size _viewSize = new Size(0.0, 0.0);
        public Size ViewSize
        {
            get { return _viewSize; }
            set { SetProperty(ref _viewSize, value); }
        }

        public event EventHandler<PanEventArgs> PanChange;

        public double PanControlOpacity
        {
            get { return _panControlOpacity; }
            set { this.SetProperty(ref _panControlOpacity, value); }
        }

        public double PanOrbOpacity
        {
            get { return _panOrbOpacity; }
            set { this.SetProperty(ref _panOrbOpacity, value); }
        }

        public double TranslateXFrom
        {
            get { return _translateXFrom; }
            set { this.SetProperty(ref _translateXFrom, value); }
        }
        public double TranslateXTo 
        {
            get { return _translateXTo; }
            set { this.SetProperty(ref _translateXTo, value); }
        }
        public double TranslateYFrom
        {
            get { return _translateYFrom; }
            set { this.SetProperty(ref _translateYFrom, value); }
        }
        public double TranslateYTo
        {
            get { return _translateYTo; }
            set { this.SetProperty(ref _translateYTo, value); }
        }
        public IPanKnobTransform PanKnobTransform
        {
            get { return _panKnobTransform; }
            private set { this.SetProperty<IPanKnobTransform>(ref _panKnobTransform, value); }
        }

        public PanKnobState State
        {
            get { return _state; }
            private set { this.SetProperty(ref _state, value); }
        }

        public bool IsPanning
        {
            get { return _isPanning; }
            private set { this.SetProperty(ref _isPanning, value); }
        }

        private readonly ICommand _showKnobCommand;
        public ICommand ShowKnobCommand { get { return _showKnobCommand; } }

        private readonly ICommand _hideKnobCommand;
        public ICommand HideKnobCommand { get { return _hideKnobCommand; } }


        // handles press&hold, double press&hold and hold release to manage knob state
        private IPointerEventConsumer _pointerEventConsumer;
        public IPointerEventConsumer PointerEventConsumer
        {
            get { return _pointerEventConsumer; }
            set { SetProperty(ref _pointerEventConsumer, value); }
        }

        public PanKnobViewModel()
        {
            this.PointerEventConsumer = new PanKnobPointerEventDispatcher();
            this.PointerEventConsumer.ConsumptionMode = ConsumptionMode.Pointer;
            this.PointerEventConsumer.ConsumedEvent += HandlePointerEvent;

            _showKnobCommand = new RelayCommand((o) => { this.PanKnobTransform = new PanKnobTransform(PanKnobTransformType.ShowKnob); });
            _hideKnobCommand = new RelayCommand((o) => { this.PanKnobTransform = new PanKnobTransform(PanKnobTransformType.HideKnob); });

            TranslateXFrom = 0.0;
            TranslateYFrom = 0.0;
            TranslateXTo = 0.0;
            TranslateYTo = 0.0;

            _lastTouchTimeStamp = 0;
            this.State = PanKnobState.Inactive;
            this.IsPanning = false;
            this.PanControlOpacity = 1.0;
            this.PanOrbOpacity = 1.0;
            _isInertiaNotProcessed = false;
            _isInertiaEnabled = false;
        }

        void HandlePointerEvent(object sender, PointerEvent e)
        {
            if(e.Inertia)
            {
                // inertia is enabled only after ManipulationInertiaStarting
                _isInertiaEnabled = true;
            }

            if (TouchEventType.Down == e.ActionType)
            {
                // click or double click
                if (_lastTouchTimeStamp != 0 && (e.TimeStamp - _lastTouchTimeStamp < GlobalConstants.MaxDoubleTapUS))
                {
                    // This is a double tap guesture so enable moving the pan control
                    _lastTouchTimeStamp = 0;
                    this.State = PanKnobState.Moving;
                }
                else
                {
                    _lastTouchTimeStamp = e.TimeStamp;
                    this.State = PanKnobState.Active;
                }

                this.PanOrbOpacity = 1.0;
                this.IsPanning = true;
                _isInertiaNotProcessed = false;
            }
            else if(TouchEventType.Up == e.ActionType)
            {
                if (_isInertiaEnabled)
                {
                    _isInertiaNotProcessed = true;
                }
                else
                {
                    this.State = PanKnobState.Inactive;
                }
                this.IsPanning = false;
            }
            else if(_isInertiaNotProcessed && PanKnobState.Inactive != this.State)
            {
                if (e.Inertia)
                {
                    this.ApplyTransform(e.Delta.X, e.Delta.Y);
                }
                else
                {
                    // completed inertia, will need another OnManipulationInertiaStarting to process again.
                    _isInertiaNotProcessed = false;
                    _isInertiaEnabled = false;
                    this.State = PanKnobState.Inactive;
                }
            } 
            else
            {
                // move or pan
                this.ApplyTransform(e.Delta.X, e.Delta.Y);
                _isInertiaNotProcessed = false;
            }
        }

        private void ApplyTransform(double x, double y)
        {
            if (PanKnobState.Active == this.State)
            {
                // pan
                if (null != PanChange)
                {
                    PanChange(this, new PanEventArgs(x, y));
                }
            }
            if (PanKnobState.Moving == this.State)
            {
                // move,  within the margins
                double panXTo = this.TranslateXTo + x;
                double panYTo = this.TranslateYTo + y;
                double borderLeft = -(this.ViewSize.Width - GlobalConstants.PanKnobWidth) / 2.0;
                double borderRight = (this.ViewSize.Width - GlobalConstants.PanKnobWidth) / 2.0;
                double borderUp = -(this.ViewSize.Height - GlobalConstants.PanKnobWidth) / 2.0;
                double borderDown = (this.ViewSize.Height - GlobalConstants.PanKnobWidth) / 2.0;

                if (panXTo < borderLeft)
                {
                    panXTo = borderLeft;
                }
                else if (panXTo > borderRight)
                {
                    panXTo = borderRight;
                }

                if (panYTo < borderUp)
                {
                    panYTo = borderUp;
                }
                else if (panYTo > borderDown)
                {
                    panYTo = borderDown;
                }

                this.TranslateXFrom = this.TranslateXTo;
                this.TranslateYFrom = this.TranslateYTo;
                this.TranslateXTo = panXTo;
                this.TranslateYTo = panYTo;
                this.PanKnobTransform = new PanKnobMoveTransform(x, y);
            }
        }

    }
}
