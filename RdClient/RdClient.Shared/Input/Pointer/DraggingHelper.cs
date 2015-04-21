using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public enum DragButton
    {
        Left,
        Right        
    }

    public class DraggingHelper
    { 
        public static void Dragging(IRemoteSessionControl control, PointerDragAction action, DragButton button, Point position)
        {

            switch (action)
            {
                case PointerDragAction.Begin:
                    switch(button)
                    {
                        case DragButton.Left:
                            control.SendMouseAction(new MouseAction(MouseEventType.LeftPress, position));
                            break;
                        case DragButton.Right:
                            control.SendMouseAction(new MouseAction(MouseEventType.RightPress, position));
                            break;
                    }
                    break;
                case PointerDragAction.Update:
                    control.SendMouseAction(new MouseAction(MouseEventType.Move, position));
                    break;
                case PointerDragAction.End:
                    switch (button)
                    {
                        case DragButton.Left:
                            control.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, position));
                            break;
                        case DragButton.Right:
                            control.SendMouseAction(new MouseAction(MouseEventType.RightRelease, position));
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
