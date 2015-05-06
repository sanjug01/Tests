using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobStateMachineEvent
    {
        public IPanKnobControl Control { get; set; }
        public IPointerEventBase Input { get; set; }
    }
}
