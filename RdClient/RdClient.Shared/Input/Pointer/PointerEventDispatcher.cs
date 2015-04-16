using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        public ConsumptionMode ConsumptionMode
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public void ConsumeEvent(IPointerEventBase pointerEvent)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
