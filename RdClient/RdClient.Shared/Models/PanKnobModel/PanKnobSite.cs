using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.Viewport;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobSite : IPanKnobSite
    {
        private ConsumptionModeType _consumptionMode;
        private ITimerFactory _timerFactory;
        ITimerFactory IPanKnobSite.TimerFactory { get { return _timerFactory; } }

        private IStateMachine<PanKnobState, PanKnobStateMachineEvent> _stateMachine;
        private PanKnobStateMachineEvent _stateMachineEvent;

        private IViewport _viewport;
        public IViewport Viewport
        {
            private get
            {
                return _viewport;
            }
            set
            {
                _viewport = value;
                if (null != _stateMachineEvent.Control)
                {
                    _stateMachineEvent.Control.Viewport = value;
                }
            }
        }

        public PanKnobSite(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;

            _stateMachine = new StateMachine<PanKnobState, PanKnobStateMachineEvent>();
            PanKnobTransitions.AddTransitions(ref _stateMachine);
            _stateMachine.SetStart(PanKnobState.Idle);

            _stateMachineEvent = new PanKnobStateMachineEvent();
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        private IPanKnob _panKnob;
        IPanKnob IPanKnobSite.PanKnob
        {
            get
            {
                return _panKnob;
            }
            set {
                _panKnob = value;
                _stateMachineEvent.Control = new PanKnobControl(value);
            }
        }

        void IPointerEventConsumer.Consume(IPointerEventBase pointerEvent)
        {
            if (_stateMachineEvent.Control != null)
            {
                _stateMachineEvent.Input = pointerEvent;
                _stateMachine.Consume(_stateMachineEvent);
                if (ConsumedEvent != null)
                {
                    ConsumedEvent(this, pointerEvent);
                }
            }
        }

        void IPointerEventConsumer.Reset()
        {
            _stateMachine.SetStart(PanKnobState.Idle);

            Point center = new Point(0, 0);           

            ((IPanKnobSite)this).PanKnob.Position = center;
        }

        void IPanKnobSite.OnConsumptionModeChanged(object sender, ConsumptionModeType consumptionMode)
        {
            _consumptionMode = consumptionMode;
            if (false == (_consumptionMode == ConsumptionModeType.DirectTouch || _consumptionMode == ConsumptionModeType.MultiTouch))
            {
                if (null != _panKnob)
                {
                    _panKnob.IsVisible = false;
                }
            }
        }

        void IPanKnobSite.OnViewportChanged(object sender, EventArgs e)
        {
            if(this.Viewport.ZoomFactor > 1.0 && (_consumptionMode == ConsumptionModeType.DirectTouch || _consumptionMode == ConsumptionModeType.MultiTouch))
            {
                if (null != _panKnob)
                {
                    _panKnob.IsVisible = true;
                }
            }
            else
            {
                if (null != _panKnob)
                {
                    _panKnob.IsVisible = false;
                }
            }

            // make sure the PanKnob doesn't fall outside the viewport when displaying the on screen keyboard
            Point current = _panKnob.Position;
            if(_panKnob.IsVisible)
            {
                if (current.Y < -(_viewport.Size.Height / 2) + (_panKnob.Size.Height / 2))
                {
                    current.Y = -(_viewport.Size.Height / 2) + (_panKnob.Size.Height / 2);
                }
                else if (current.Y + _panKnob.Size.Height / 2 > _viewport.Size.Height / 2)
                {
                    current.Y = _viewport.Size.Height / 2 - (_panKnob.Size.Height / 2);
                }
                _panKnob.Position = current;
            }
        }
    }
}
