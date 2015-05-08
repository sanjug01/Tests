using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Input.Pointer
{
    public enum DragOrientation
    {
        Horizontal,
        Vertical,
        Unknown
    }

    public enum PointerModeState
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

    public enum DirectModeState
    {
        Idle,
        LeftDragging,
        Holding,
        RightDragging
    }

    public class PointerStateMachineEvent
    {
        public IPointerEventBase Input { get; set; }
        public IPointTracker Tracker { get; set; }
        public DoubleClickTimer Timer { get; set; }
        public IPointerControl Control { get; set; }
    }
}
