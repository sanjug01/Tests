using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Mouse
{
    public class ClickTouchContext : TouchContext
    {        
        public ClickTouchContext(ITimer timer, 
                                 IPointerManipulator manipulator, 
                                 IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine) : base(timer, manipulator, stateMachine)
        {

        }
        public override void MouseLeftClick(PointerEvent pointerEvent)
        {
            MouseMove(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            PointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public override void MouseRightClick(PointerEvent pointerEvent)
        {
            MouseMove(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.RightPress);
            PointerManipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public override void UpdateCursorPosition(PointerEvent pointerEvent)
        {
            this.PointerManipulator.MousePosition = pointerEvent.Position;
        }

        public override bool MoveThresholdExceeded(PointerEvent pointerEvent)
        {
            return false;
        }

        public override int NumberOfContacts(PointerEvent pointerEvent)
        {
            return base.NumberOfContacts(pointerEvent);
        }
    }
}
