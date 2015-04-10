using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerConsumer : IPointerEventConsumer
    {
        private IPointerContext _context;
        private IStateMachine<PointerState, StateEvent<PointerEventOld, IPointerContext>> _stateMachine;
        private StateEvent<PointerEventOld, IPointerContext> _stateEvent;

        public event EventHandler<PointerEventOld> ConsumedEvent;

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }

        public PointerConsumer(
            IPointerContext context,
            IStateMachine<PointerState, StateEvent<PointerEventOld, IPointerContext>> stateMachine)
        {
            _context = context;
            _stateEvent = new StateEvent<PointerEventOld, IPointerContext>() { Input = null, Context = _context };
            _stateMachine = stateMachine;
        }

        public void ConsumeEvent(PointerEventOld pointerEvent)
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
