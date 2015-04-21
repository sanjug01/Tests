using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeTransitionConditions
    {
        public static bool Idle_LeftDragging_Condition(StateMachineEvent obj)
        {
            return 
                obj.Input.Action == PointerEventAction.ManipulationStarted && 
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void Idle_LeftDragging_Action(StateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool Idle_Holding_Condition(StateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.HoldingStarted;
        }

        public static void Idle_Holding_Action(StateMachineEvent obj)
        {
        }

        public static bool LeftDragging_LeftDragging_Condition(StateMachineEvent obj)
        {
            return 
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void LeftDragging_LeftDragging_Action(StateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool LeftDragging_Idle_Condition(StateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void LeftDragging_Idle_Action(StateMachineEvent obj)
        {
            obj.Control.LeftDrag(PointerDragAction.End, new Point(0, 0), obj.Input.Position);
        }
        public static bool Holding_RightDragging_Condition(StateMachineEvent obj)
        {
            return
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void Holding_RightDragging_Action(StateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.Begin, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool RightDragging_RightDragging_Condition(StateMachineEvent obj)
        {
            return
                obj.Input.Action == PointerEventAction.ManipulationDelta &&
                RdMath.Distance((obj.Input as IManipulationRoutedEventProperties).Delta.Translation) > GlobalConstants.TouchMoveThreshold;
        }

        public static void RightDragging_RightDragging_Action(StateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.Update, ((IManipulationRoutedEventProperties)obj.Input).Delta.Translation, obj.Input.Position);
        }

        public static bool RightDragging_Idle_Condition(StateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void RightDragging_Idle_Action(StateMachineEvent obj)
        {
            obj.Control.RightDrag(PointerDragAction.End, new Point(0, 0), obj.Input.Position);
        }

        public static bool Holding_Idle_Condition(StateMachineEvent obj)
        {
            return obj.Input.Action == PointerEventAction.HoldingCompleted;
        }

        public static void Holding_Idle_Action(StateMachineEvent obj)
        {
            obj.Control.RightClick(obj.Input.Position);
        }
    }
}
