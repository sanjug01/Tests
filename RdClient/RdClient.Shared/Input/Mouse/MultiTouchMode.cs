using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Mouse
{
    public class MultiTouchMode : IPointerEventConsumer
    {
        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set { _consumptionMode = value; }
        }

        private Dictionary<uint, PointerEvent> _trackedTouches;

        public MultiTouchMode()
        {
            _trackedTouches = new Dictionary<uint, PointerEvent>();
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            switch(pointerEvent.ActionType)
            {
                case PointerActionType.Down:
                    _trackedTouches[pointerEvent.PointerId] = pointerEvent;
                    break;
                case PointerActionType.Up:
                    if(_trackedTouches.ContainsKey(pointerEvent.PointerId))
                    {
                        _trackedTouches.Remove(pointerEvent.PointerId);
                    }
                    break;
                default:
                    break;
            }
        }

        public void Reset()
        {
            _trackedTouches.Clear();
        }

    }
}
