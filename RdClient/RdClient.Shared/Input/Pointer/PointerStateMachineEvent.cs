using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Input.Pointer
{
    public enum DragOrientation
    {
        Horizontal,
        Vertical,
        Unknown
    }

    public enum PointerState
    {
        Idle,
        LeftDown,
        LeftDoubleDown,
        RightDown,
        RightDoubleDown,
        Move,
        LeftDrag,
        RightDrag,
        Scroll,
        ZoomPan
    }


    public class StateMachineEvent
    {
        public IPointerEventBase Input { get; set; }
        public IPointTracker Tracker { get; set; }
        public DoubleClickTimer Timer { get; set; }
        public IPointerControl Control { get; set; }
    }
}
