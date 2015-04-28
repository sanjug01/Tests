using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Models.PanKnobModel
{
    public enum PanKnobState
    {
        Idle,
        Panning,
        Dragging,
        Zooming
    }


    public class PanKnobTransitions
    {
        public static void AddTransitions(ref IStateMachine<PanKnobState, PanKnobStateMachineEvent> stateMachine)
        {
            stateMachine.AddTransition(
                PanKnobState.Idle, PanKnobState.Panning,
                PanKnobTransitionConditions.Idle_Panning_Condition,
                PanKnobTransitionConditions.Idle_Panning_Action);

            stateMachine.AddTransition(
                PanKnobState.Panning, PanKnobState.Panning,
                PanKnobTransitionConditions.Panning_Panning_Condition,
                PanKnobTransitionConditions.Panning_Panning_Action);

            stateMachine.AddTransition(
                PanKnobState.Panning, PanKnobState.Idle,
                PanKnobTransitionConditions.Panning_Idle_Condition,
                PanKnobTransitionConditions.Panning_Idle_Action);

            stateMachine.AddTransition(
                PanKnobState.Idle, PanKnobState.Dragging,
                PanKnobTransitionConditions.Idle_Dragging_Condition,
                PanKnobTransitionConditions.Idle_Dragging_Action);

            stateMachine.AddTransition(
                PanKnobState.Dragging, PanKnobState.Dragging,
                PanKnobTransitionConditions.Dragging_Dragging_Condition,
                PanKnobTransitionConditions.Dragging_Dragging_Action);

            stateMachine.AddTransition(
                PanKnobState.Dragging, PanKnobState.Idle,
                PanKnobTransitionConditions.Dragging_Idle_Condition,
                PanKnobTransitionConditions.Dragging_Idle_Action);

            stateMachine.AddTransition(
                PanKnobState.Idle, PanKnobState.Zooming,
                PanKnobTransitionConditions.Idle_Zooming_Condition,
                PanKnobTransitionConditions.Idle_Zooming_Action);

            stateMachine.AddTransition(
                PanKnobState.Zooming, PanKnobState.Zooming,
                PanKnobTransitionConditions.Zooming_Zooming_Condition,
                PanKnobTransitionConditions.Zooming_Zooming_Action);

            stateMachine.AddTransition(
                PanKnobState.Zooming, PanKnobState.Idle,
                PanKnobTransitionConditions.Zooming_Idle_Condition,
                PanKnobTransitionConditions.Zooming_Idle_Action);
        }
    }
}
