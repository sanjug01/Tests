using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeConsumer : IPointerEventConsumer
    {
        private IPointerControl _control;
        private IPointerPosition _pointerPosition;
        private IStateMachine<DirectModeState, StateMachineEvent> _stateMachine;
        StateMachineEvent _stateMachineEvent;        

        public DirectModeConsumer(IPointerControl control, IPointerPosition pointerPosition)
        {
            _control = control;
            _pointerPosition = pointerPosition;

            _stateMachine = new StateMachine<DirectModeState, StateMachineEvent>();
            _stateMachineEvent = new StateMachineEvent() { Input = null, Tracker = null, Timer = null, Control = control };
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

            if(pointerEvent.Action == PointerEventAction.Tapped)
            {
                IGestureRoutedEventProperties grep = (IGestureRoutedEventProperties) pointerEvent;
                int i;
                for(i = 0; i < grep.Count; i++)
                {
                    _control.LeftClick(_pointerPosition.SessionPosition);
                }                
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
