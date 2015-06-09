using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.Viewport;
using System;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobSite : IPanKnobSite
    {
        private ITimerFactory _timerFactory;
        ITimerFactory IPanKnobSite.TimerFactory { get { return _timerFactory; } }

        private IStateMachine<PanKnobState, PanKnobStateMachineEvent> _stateMachine;
        private PanKnobStateMachineEvent _stateMachineEvent;

        public IViewport Viewport
        {
            set
            {
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
                if(ConsumedEvent != null)
                {
                    ConsumedEvent(this, pointerEvent);
                }
            }
        }

        void IPointerEventConsumer.Reset()
        {
            _stateMachine.SetStart(PanKnobState.Idle);
        }

        void IPanKnobSite.OnConsumptionModeChanged(object sender, ConsumptionModeType consumptionMode)
        {
            if(consumptionMode == ConsumptionModeType.DirectTouch || consumptionMode == ConsumptionModeType.MultiTouch)
            {
                if(null!=_panKnob)
                    _panKnob.IsVisible = true;
            }
            else
            {
                if(null!=_panKnob)
                    _panKnob.IsVisible = false;
            }
        }
    }
}
