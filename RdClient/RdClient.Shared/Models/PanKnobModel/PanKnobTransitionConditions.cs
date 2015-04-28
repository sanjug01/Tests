using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Models.PanKnobModel
{
    public class PanKnobTransitionConditions
    {
        public static bool Idle_Panning_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationDelta;
        }

        public static void Idle_Panning_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Panning_Panning_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationDelta;
        }

        public static void Panning_Panning_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Panning_Idle_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Panning_Idle_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Idle_Dragging_Condition(PanKnobStateMachineEvent o)
        {
            return
                o.Input.Action == PointerEventAction.Tapped &&
                ((IGestureRoutedEventProperties)o.Input).Count > 1;
        }

        public static void Idle_Dragging_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Dragging_Dragging_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationDelta;
        }

        public static void Dragging_Dragging_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Dragging_Idle_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Dragging_Idle_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Idle_Zooming_Condition(PanKnobStateMachineEvent o)
        {
            return
                o.Input.Action == PointerEventAction.ManipulationDelta &&
                ((IManipulationRoutedEventProperties)o.Input).Delta.Expansion > GlobalConstants.TouchZoomDeltaThreshold;
        }

        public static void Idle_Zooming_Action(PanKnobStateMachineEvent o)
        {
            
        }

        public static bool Zooming_Zooming_Condition(PanKnobStateMachineEvent o)
        {
            return
                o.Input.Action == PointerEventAction.ManipulationDelta &&
                ((IManipulationRoutedEventProperties)o.Input).Delta.Expansion > GlobalConstants.TouchZoomDeltaThreshold;
        }

        public static void Zooming_Zooming_Action(PanKnobStateMachineEvent o)
        {

        }

        public static bool Zooming_Idle_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Zooming_Idle_Action(PanKnobStateMachineEvent o)
        {
            
        }
    }
}
