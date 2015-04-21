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

        private ConsumptionMode _consumptionMode;
        ConsumptionMode IPointerEventConsumer.ConsumptionMode
        {
            set
            {
                _consumptionMode = value;
            }
        }


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
                _pointerPosition.PointerPosition = pointerEvent.Position;
            }

            if(pointerEvent.Action == PointerEventAction.Tapped)
            {
                IGestureRoutedEventProperties grep = (IGestureRoutedEventProperties) pointerEvent;

                if(grep.Action == PointerEventAction.Tapped)
                {
                    int i;
                    for(i = 0; i < grep.Count; i++)
                    {
                        _control.LeftClick(grep.Position);
                    }
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

        void IPointerEventConsumer.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
