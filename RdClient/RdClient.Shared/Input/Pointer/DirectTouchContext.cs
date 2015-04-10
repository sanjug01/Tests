using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectTouchContext : TouchContext
    {        
        public DirectTouchContext(ITimer timer, 
                                 IPointerManipulator manipulator, 
                                 IStateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>> stateMachine) : base(timer, manipulator, stateMachine)
        {

        }
        public override void MouseLeftClick(PointerEventOld pointerEvent)
        {
            MouseMove(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            PointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public override void MouseRightClick(PointerEventOld pointerEvent)
        {
            MouseMove(pointerEvent);
            PointerManipulator.SendMouseAction(MouseEventType.RightPress);
            PointerManipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public override void UpdateCursorPosition(PointerEventOld pointerEvent)
        {
            this.PointerManipulator.MousePosition = pointerEvent.Position;
        }

        public override int NumberOfContacts(PointerEventOld pointerEvent)
        {
            return base.NumberOfContacts(pointerEvent);
        }
    }
}
