using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeTransitions
    {
        public static void AddTransitions(ref IStateMachine<DirectModeState, PointerStateMachineEvent> stateMachine)
        {
            stateMachine.AddTransition(
                DirectModeState.Idle, DirectModeState.LeftDragging,
                DirectModeTransitionConditions.Idle_LeftDragging_Condition,
                DirectModeTransitionConditions.Idle_LeftDragging_Action);

            stateMachine.AddTransition(
                DirectModeState.LeftDragging, DirectModeState.LeftDragging,
                DirectModeTransitionConditions.LeftDragging_LeftDragging_Condition, 
                DirectModeTransitionConditions.LeftDragging_LeftDragging_Action);

            stateMachine.AddTransition(
                DirectModeState.LeftDragging, DirectModeState.Idle,
                DirectModeTransitionConditions.LeftDragging_Idle_Condition,
                DirectModeTransitionConditions.LeftDragging_Idle_Action);

            stateMachine.AddTransition(
                DirectModeState.Idle, DirectModeState.Holding,
                DirectModeTransitionConditions.Idle_Holding_Condition,
                DirectModeTransitionConditions.Idle_Holding_Action);

            stateMachine.AddTransition(
                DirectModeState.Holding, DirectModeState.RightDragging,
                DirectModeTransitionConditions.Holding_RightDragging_Condition,
                DirectModeTransitionConditions.Holding_RightDragging_Action);

            stateMachine.AddTransition(
                DirectModeState.RightDragging, DirectModeState.RightDragging,
                DirectModeTransitionConditions.RightDragging_RightDragging_Condition, 
                DirectModeTransitionConditions.RightDragging_RightDragging_Action);

            stateMachine.AddTransition(
                DirectModeState.RightDragging, DirectModeState.Idle,
                DirectModeTransitionConditions.RightDragging_Idle_Condition, 
                DirectModeTransitionConditions.RightDragging_Idle_Action);

            stateMachine.AddTransition(
                DirectModeState.Holding, DirectModeState.Idle,
                DirectModeTransitionConditions.Holding_Idle_Condition,
                DirectModeTransitionConditions.Holding_Idle_Action);

        }
    }
}