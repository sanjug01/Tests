using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.Input.Mouse;

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
            this.State = PanKnobState.Disabled;
            this.IsPanning = false;
            this.PanControlOpacity = 1.0;
            this.PanOrbOpacity = 1.0;
        }

        void HandlePointerEvent(object sender, PointerEvent e)
        {
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
                    this.State = PanKnobState.Enabled;
                }

                this.PanOrbOpacity = 1.0;
                this.IsPanning = true;
            }
            else if(TouchEventType.Up == e.ActionType)
            {
                // release
                if(e.Inertia)
                {
                    this.ApplyTransform(e.Delta.X, e.Delta.Y);
                }
                this.State = PanKnobState.Disabled;
                this.IsPanning = false;
            }
            else
            {
                // move or pan
                this.ApplyTransform(e.Delta.X, e.Delta.Y);
            }
        }

        private void ApplyTransform(double x, double y)
        {
            if (PanKnobState.Enabled == this.State)
            {
                // pan
                PanChange.Invoke(this, new PanEventArgs(x, y));
            }
            if (PanKnobState.Moving == this.State)
            {
                // move
                double panXTo = this.TranslateXTo + x;
                double panYTo = this.TranslateYTo + y;

                this.TranslateXFrom = this.TranslateXTo;
                this.TranslateYFrom = this.TranslateYTo;
                this.TranslateXTo = panXTo;
                this.TranslateYTo = panYTo;
                this.PanKnobTransform = new PanKnobMoveTransform(x, y);
            }
        }

    }
}
