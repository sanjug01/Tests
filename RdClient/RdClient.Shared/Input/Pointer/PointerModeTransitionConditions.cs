
using RdClient.Shared.Helpers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerModeTransitionConditions
    {
        public static void GoTo_Idle_Action(StateMachineEvent e)
        {
            e.Timer.Stop();
            e.Tracker.Reset();
        }

        public static bool Idle_LeftDown_Condition(StateMachineEvent e)
        {
            return
                e.Input is IPointerRoutedEventProperties &&
                e.Input.Action == PointerEventAction.PointerPressed &&
                e.Tracker.Contacts == 1 && 
                e.Timer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == true;
        }
        public static void Idle_LeftDown_Action(StateMachineEvent e)
        {            
        }

        public static bool LeftDown_Idle_Condition(StateMachineEvent e)
        {
            return 
                e.Input is IPointerRoutedEventProperties &&
                e.Tracker.Contacts == 0;
        }
        public static void LeftDown_Idle_Action(StateMachineEvent e)
        {
            if(e.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick))
            {
                e.Timer.Reset(DoubleClickTimer.ClickTimerType.LeftClick, new PointerRoutedEventArgsCopy((IPointerRoutedEventProperties)e.Input));
            }
        }

        public static bool Idle_LeftDoubleDown_Condition(StateMachineEvent e)
        {
            return
                e.Input is IPointerRoutedEventProperties &&
                e.Tracker.Contacts == 1 &&
                e.Timer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == false;
        }
        public static void Idle_LeftDoubleDown_Action(StateMachineEvent e)
        {
            e.Timer.Stop();
        }

        public static bool LeftDoubleDown_Idle_Condition(StateMachineEvent e)
        {
            return
                e.Input is IPointerRoutedEventProperties &&
                e.Tracker.Contacts == 0;
        }
        public static void LeftDoubleDown_Idle_Action(StateMachineEvent e)
        {
            IPointerRoutedEventProperties prep = (IPointerRoutedEventProperties)e.Input;
            e.Control.LeftClick(prep);
            e.Control.LeftClick(prep);
        }

        public static bool LeftDown_RightDown_Condition(StateMachineEvent e)
        {
            return 
                e.Input is IPointerRoutedEventProperties && 
                e.Tracker.Contacts == 2 && 
                e.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == true;
        }
        public static void LeftDown_RightDown_Action(StateMachineEvent e)
        {
        }

        public static bool RightDown_Idle_Condition(StateMachineEvent e)
        {
            return
                e.Input is IPointerRoutedEventProperties &&
                e.Tracker.Contacts < 2;
        }
        public static void RightDown_Idle_Action(StateMachineEvent e)
        {
            e.Timer.Reset(DoubleClickTimer.ClickTimerType.RightClick, new PointerRoutedEventArgsCopy((IPointerRoutedEventProperties)e.Input));
        }

        public static bool LeftDown_RightDoubleDown_Condition(StateMachineEvent e)
        {
            return 
                e.Input is IPointerRoutedEventProperties && 
                e.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == false && 
                e.Tracker.Contacts == 2;
        }
        public static void LeftDown_RightDoubleDown_Action(StateMachineEvent e)
        {
            e.Timer.Stop();
        }

        public static bool RightDoubleDown_Idle_Condition(StateMachineEvent e)
        {
            return e.Tracker.Contacts == 0;
        }
        public static void RightDoubleDown_Idle_Action(StateMachineEvent e)
        {
            GoTo_Idle_Action(e);
        }

        public static bool LeftDown_Move_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationStarted &&
                RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Cummulative.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void LeftDown_Move_Action(StateMachineEvent e)
        {
            e.Timer.Stop();
            e.Control.Move(((IManipulationRoutedEventProperties)e.Input).Cummulative.Translation);
        }

        public static bool Move_Move_Condition(StateMachineEvent e)
        {
            return
                (e.Input.Action == PointerEventAction.ManipulationDelta &&
                    RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Delta.Translation) > GlobalConstants.TouchMoveThreshold);
        }
        public static void Move_Move_Action(StateMachineEvent e)
        {
            if(e.Input.Action == PointerEventAction.ManipulationDelta)
            {
                e.Control.Move(((IManipulationRoutedEventProperties)e.Input).Delta.Translation);
            }
        }

        public static bool Move_LeftDown_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.PointerPressed &&
                e.Tracker.Contacts == 1;
        }

        public static void Move_LeftDown_Action(StateMachineEvent e)
        {

        }

        public static bool Move_Idle_Condition(StateMachineEvent e)
        {
            return e.Input.Action == PointerEventAction.ManipulationCompleted;
        }
        public static void Move_Idle_Action(StateMachineEvent e)
        {
            GoTo_Idle_Action(e);
        }

        public static bool LeftDoubleDown_LeftDrag_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }
        public static void LeftDoubleDown_LeftDrag_Action(StateMachineEvent e)
        {
            e.Control.LeftDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)e.Input).Delta.Translation);
        }

        public static bool LeftDrag_LeftDrag_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void LeftDrag_LeftDrag_Action(StateMachineEvent e)
        {
            e.Control.LeftDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)e.Input).Delta.Translation);
        }
        public static bool LeftDrag_Idle_Condition(StateMachineEvent e)
        {
            return 
                e.Input.Action == PointerEventAction.ManipulationInertiaStarting || 
                e.Input.Action == PointerEventAction.ManipulationCompleted;
        }
        public static void LeftDrag_Idle_Action(StateMachineEvent e)
        {
            e.Control.LeftDrag(PointerDragAction.End, new Point(0, 0));
            GoTo_Idle_Action(e);
        }

        public static bool RightDoubleDown_RightDrag_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }
        public static void RightDoubleDown_RightDrag_Action(StateMachineEvent e)
        {
            e.Control.RightDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)e.Input).Delta.Translation);
        }

        public static bool RightDrag_RightDrag_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance(((IManipulationRoutedEventProperties)e.Input).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void RightDrag_RightDrag_Action(StateMachineEvent e)
        {
            e.Control.RightDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)e.Input).Delta.Translation);
        }

        public static bool RightDrag_Idle_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ManipulationInertiaStarting ||
                e.Input.Action == PointerEventAction.ManipulationCompleted;
        }
        public static void RightDrag_Idle_Action(StateMachineEvent e)
        {
            e.Control.RightDrag(PointerDragAction.End, new Point(0, 0));
            GoTo_Idle_Action(e);
        }

        public static bool RightDown_Scroll_Condition(StateMachineEvent e)
        {
            return 
                e.Input.Action == PointerEventAction.ZoomScrollStarted &&
                (
                    ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.Scroll ||
                    ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.HScroll);
        }

        public static void RightDown_Scroll_Action(StateMachineEvent e)
        {
            if(((IZoomScrollEvent) e.Input).Type == ZoomScrollType.Scroll)
            {
                e.Control.Scroll(((IZoomScrollEvent)e.Input).Delta.Translation.Y);
            }
            else if (((IZoomScrollEvent)e.Input).Type == ZoomScrollType.HScroll)
            {
                e.Control.HScroll(((IZoomScrollEvent)e.Input).Delta.Translation.X);
            }
        }

        public static bool Scroll_Scroll_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ZoomScrollUpdating &&
                (
                    ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.Scroll ||
                    ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.HScroll);
        }

        public static void Scroll_Scroll_Action(StateMachineEvent e)
        {
            if (((IZoomScrollEvent)e.Input).Type == ZoomScrollType.Scroll)
            {
                e.Control.Scroll(((IZoomScrollEvent)e.Input).Delta.Translation.Y);
            }
            else if (((IZoomScrollEvent)e.Input).Type == ZoomScrollType.HScroll)
            {
                e.Control.HScroll(((IZoomScrollEvent)e.Input).Delta.Translation.X);
            }
        }

        public static bool Scroll_Idle_Condition(StateMachineEvent e)
        {
            return e.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Scroll_Idle_Action(StateMachineEvent e)
        {
            GoTo_Idle_Action(e);
        }

        public static bool RightDown_ZoomPan_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ZoomScrollStarted &&
                ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.ZoomPan;
        }

        public static void RightDown_ZoomPan_Action(StateMachineEvent e)
        {
            e.Control.ZoomPan(e.Tracker.Center, ((IZoomScrollEvent)e.Input).Delta.Translation, ((IZoomScrollEvent)e.Input).Delta.Scale);
        }

        public static bool ZoomPan_ZoomPan_Condition(StateMachineEvent e)
        {
            return
                e.Input.Action == PointerEventAction.ZoomScrollUpdating &&
                ((IZoomScrollEvent)e.Input).Type == ZoomScrollType.ZoomPan;
        }

        public static void ZoomPan_ZoomPan_Action(StateMachineEvent e)
        {
            e.Control.ZoomPan(e.Tracker.Center, ((IZoomScrollEvent)e.Input).Delta.Translation, ((IZoomScrollEvent)e.Input).Delta.Scale);
        }

        public static bool ZoomPan_Idle_Condition(StateMachineEvent e)
        {
            return e.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void ZoomPan_Idle_Action(StateMachineEvent e)
        {
            GoTo_Idle_Action(e);
        }
    }
}
