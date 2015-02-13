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

    public class PanKnobTransform : IPanKnobTransform, IPointerManipulator
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

        public ConsumptionMode ConsumptionMode
        {
            set { throw new NotImplementedException(); }
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }


    public sealed class PanKnobViewModel : MutableObject
    {
        private IPanKnobTransform _panKnobTransform;

        private double _translateXFrom;
        private double _translateXTo;
        private double _translateYFrom;
        private double _translateYTo;

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

        private readonly ICommand _moveKnobCommand;
        public ICommand MoveKnobCommand { get { return _moveKnobCommand; } }

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
            this.PointerEventConsumer.ConsumedEvent += (s, o) =>
            {
                // TODO
            };

            _moveKnobCommand = new RelayCommand(new Action<object>(MovePanKnob));

            TranslateXFrom = 0.0;
            TranslateYFrom = 0.0;
            TranslateXTo = 0.0;
            TranslateYTo = 0.0;
        }

        private void MovePanKnob(object o)
        {
            PanKnobMoveTransform panTransform = (o as PanKnobMoveTransform);
            if (null != panTransform)
            {
                this.ApplyMoveTransform(panTransform.DeltaX, panTransform.DeltaY);
                this.PanKnobTransform = new PanKnobMoveTransform(panTransform.DeltaX, panTransform.DeltaY);
            }
        }

        private void ApplyMoveTransform(double x, double y)
        {
            double panXTo = this.TranslateXTo + x;
            double panYTo = this.TranslateYTo + y;

            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;
            this.TranslateXTo = panXTo;
            this.TranslateYTo = panYTo;
        }
    }
}
