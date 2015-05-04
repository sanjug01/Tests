using System;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using System.Diagnostics;
using RdClient.Shared.Input.Recognizers;

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
                _stateMachineEvent.Control.Viewport = value;
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
                _panKnob.IsVisible = true;
            }
            else
            {
                _panKnob.IsVisible = false;
            }
        }
    }
}
