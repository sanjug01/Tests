using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class DirectModeControl : PointerModeControl
    {
        public DirectModeControl(IPointerContext context, IPointerManipulator manipulator, IRenderingPanel panel) : base(context, manipulator, panel)
        {

        }

        public override void MouseLeftClick(PointerEvent pointerEvent)
        {
            MouseMove(pointerEvent);
            Manipulator.SendMouseAction(MouseEventType.LeftPress);
            Manipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public override void MouseRightClick(PointerEvent pointerEvent)
        {
            MouseMove(pointerEvent);
            Manipulator.SendMouseAction(MouseEventType.RightPress);
            Manipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public override void UpdateCursorPosition(PointerEvent pointerEvent)
        {
            Manipulator.MousePosition = pointerEvent.Position;
        }

    }
}
