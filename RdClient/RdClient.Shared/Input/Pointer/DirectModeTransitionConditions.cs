using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Recognizers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeTransitionConditions
    {
        public static bool Idle_LeftDragging_Condition(PointerStateMachineEvent obj)
        {
            return 
                obj.Input.Action == PointerEventAction.ManipulationStarted && 
                (obj.Input as IManipulationRoutedEventProperties).IsInertial == false &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void Idle_LeftDragging_Action(PointerStateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool Idle_Holding_Condition(PointerStateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.Tapped && ((ITapEvent) obj.Input).Type == TapEventType.HoldingStarted;
        }

        public static void Idle_Holding_Action(PointerStateMachineEvent obj)
        {
        }

        public static bool LeftDragging_LeftDragging_Condition(PointerStateMachineEvent obj)
        {
            return 
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                (obj.Input as IManipulationRoutedEventProperties).IsInertial == false &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void LeftDragging_LeftDragging_Action(PointerStateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool LeftDragging_Idle_Condition(PointerStateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void LeftDragging_Idle_Action(PointerStateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.End, new Point(0, 0), obj.Input.Position);
        }
        public static bool Holding_RightDragging_Condition(PointerStateMachineEvent obj)
        {
            return
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                (obj.Input as IManipulationRoutedEventProperties).IsInertial == false &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void Holding_RightDragging_Action(PointerStateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool RightDragging_RightDragging_Condition(PointerStateMachineEvent obj)
        {
            return
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                (obj.Input as IManipulationRoutedEventProperties).IsInertial == false &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void RightDragging_RightDragging_Action(PointerStateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool RightDragging_Idle_Condition(PointerStateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void RightDragging_Idle_Action(PointerStateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.End, new Point(0, 0), obj.Input.Position);
        }

        public static bool Holding_Idle_Condition(PointerStateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.Tapped && ((ITapEvent)obj.Input).Type == TapEventType.HoldingCompleted;
        }

        public static void Holding_Idle_Action(PointerStateMachineEvent obj)
        {
            obj.Control.RightClick(obj.PointerPosition.SessionPosition);
        }
    }
}
