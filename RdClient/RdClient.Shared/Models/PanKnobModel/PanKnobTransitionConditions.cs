using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Input.Recognizers;
using System;
using Windows.Foundation;

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
            Point delta = ((IManipulationRoutedEventProperties)o.Input).Delta.Translation;
            o.Control.Pan(delta.X, delta.Y);
        }        

        public static bool Panning_Panning_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationDelta;
        }

        public static void Panning_Panning_Action(PanKnobStateMachineEvent o)
        {
            Point delta = ((IManipulationRoutedEventProperties)o.Input).Delta.Translation;
            o.Control.Pan(delta.X, delta.Y);
        }

        public static bool Panning_Idle_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Panning_Idle_Action(PanKnobStateMachineEvent o)
        {
            o.Control.Complete();
        }

        public static bool Idle_Dragging_Condition(PanKnobStateMachineEvent o)
        {
            return
                o.Input.Action == PointerEventAction.Tapped &&
                (((ITapEvent)o.Input).Type == TapEventType.TapMovingStarted ||
                 ((ITapEvent)o.Input).Type == TapEventType.TapHoldingStarted);
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
            Point delta = ((IManipulationRoutedEventProperties)o.Input).Delta.Translation;
            o.Control.Move(delta.X, delta.Y);
        }

        public static bool Dragging_Idle_Condition(PanKnobStateMachineEvent o)
        {
            return o.Input.Action == PointerEventAction.ManipulationCompleted;
        }

        public static void Dragging_Idle_Action(PanKnobStateMachineEvent o)
        {
            o.Control.Complete();
        }
    }
}
