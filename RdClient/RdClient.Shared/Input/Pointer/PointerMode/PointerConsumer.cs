﻿using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerConsumer : IPointerEventConsumer
    {
        private IPointerContext _context;
        private IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> _stateMachine;
        private StateEvent<PointerEvent, IPointerContext> _stateEvent;

        public event EventHandler<PointerEvent> ConsumedEvent;

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }

        public PointerConsumer(
            IPointerContext context,
            IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> stateMachine)
        {
            _context = context;
            _stateEvent = new StateEvent<PointerEvent, IPointerContext>() { Input = null, Context = _context };
            _stateMachine = stateMachine;
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            _stateEvent.Input = pointerEvent;
            _stateMachine.Consume(_stateEvent);
            _context.TrackEvent(pointerEvent);
        }

        public void Reset()
        {
            _context.Reset();
        }
    }
}