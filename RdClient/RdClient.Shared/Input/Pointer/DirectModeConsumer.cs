using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Recognizers;
using System;
using System.Diagnostics;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeConsumer : IPointerEventConsumer
    {
        private IPointerControl _control;
        private IPointerPosition _pointerPosition;
        private IStateMachine<DirectModeState, PointerStateMachineEvent> _stateMachine;
        PointerStateMachineEvent _stateMachineEvent;        

        public DirectModeConsumer(IPointerControl control, IPointerPosition pointerPosition)
        {
            _control = control;
            _pointerPosition = pointerPosition;

            _stateMachine = new StateMachine<DirectModeState, PointerStateMachineEvent>();
            _stateMachineEvent = new PointerStateMachineEvent() {
                Input = null,
                Tracker = null,
                Timer = null,
                Control = control,
                PointerPosition = pointerPosition
            };
            DirectModeTransitions.AddTransitions(ref _stateMachine);

            Reset();
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        void IPointerEventConsumer.Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent.Action == PointerEventAction.PointerPressed)
            {
                _pointerPosition.ViewportPosition = pointerEvent.Position;
            }

            if (pointerEvent.Action == PointerEventAction.Tapped && ((ITapEvent)pointerEvent).Type == TapEventType.Tap)
            {
                _control.LeftClick(_pointerPosition.SessionPosition);
            }
            else if (pointerEvent.Action == PointerEventAction.Tapped && ((ITapEvent)pointerEvent).Type == TapEventType.DoubleTap)
            {
                _control.LeftClick(_pointerPosition.SessionPosition);
                _control.LeftClick(_pointerPosition.SessionPosition);            
            }
            else
            {
                _stateMachineEvent.Input = pointerEvent;
                _stateMachine.Consume(_stateMachineEvent);
            }

            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _stateMachine.SetStart(DirectModeState.Idle);      
        }
    }
}
