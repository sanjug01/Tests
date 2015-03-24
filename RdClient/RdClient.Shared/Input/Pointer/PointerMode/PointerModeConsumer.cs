using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeConsumer : IPointerEventConsumer
    {

        public event EventHandler<PointerEvent> ConsumedEvent;

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
