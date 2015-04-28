using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerModeTransitions
    {
        public static void AddTransitions(ref IStateMachine<PointerModeState, PointerStateMachineEvent> stateMachine)
        {
            stateMachine.AddTransition(
                PointerModeState.Idle, PointerModeState.LeftDown, 
                PointerModeTransitionConditions.Idle_LeftDown_Condition, 
                PointerModeTransitionConditions.Idle_LeftDown_Action);

            stateMachine.AddTransition(
                PointerModeState.Idle, PointerModeState.LeftDoubleDown,
                PointerModeTransitionConditions.Idle_LeftDoubleDown_Condition,
                PointerModeTransitionConditions.Idle_LeftDoubleDown_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDoubleDown, PointerModeState.Idle,
                PointerModeTransitionConditions.LeftDoubleDown_Idle_Condition, 
                PointerModeTransitionConditions.LeftDoubleDown_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDown, PointerModeState.RightDown,
                PointerModeTransitionConditions.LeftDown_RightDown_Condition,
                PointerModeTransitionConditions.LeftDown_RightDown_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDown, PointerModeState.RightDoubleDown,
                PointerModeTransitionConditions.LeftDown_RightDoubleDown_Condition,
                PointerModeTransitionConditions.LeftDown_RightDoubleDown_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDoubleDown, PointerModeState.Idle,
                PointerModeTransitionConditions.RightDoubleDown_Idle_Condition,
                PointerModeTransitionConditions.RightDoubleDown_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDown, PointerModeState.Move,
                PointerModeTransitionConditions.LeftDown_Move_Condition, 
                PointerModeTransitionConditions.LeftDown_Move_Action);

            stateMachine.AddTransition(
                PointerModeState.Move, PointerModeState.Move,
                PointerModeTransitionConditions.Move_Move_Condition,
                PointerModeTransitionConditions.Move_Move_Action);

            stateMachine.AddTransition(
                PointerModeState.Move, PointerModeState.LeftDown,
                PointerModeTransitionConditions.Move_LeftDown_Condition,
                PointerModeTransitionConditions.Move_LeftDown_Action);

            stateMachine.AddTransition(
                PointerModeState.Move, PointerModeState.Idle,
                PointerModeTransitionConditions.Move_Idle_Condition,
                PointerModeTransitionConditions.Move_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDown, PointerModeState.Idle,
                PointerModeTransitionConditions.LeftDown_Idle_Condition,
                PointerModeTransitionConditions.LeftDown_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDown, PointerModeState.Scroll,
                PointerModeTransitionConditions.RightDown_Scroll_Condition,
                PointerModeTransitionConditions.RightDown_Scroll_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDown, PointerModeState.ZoomPan,
                PointerModeTransitionConditions.RightDown_ZoomPan_Condition,
                PointerModeTransitionConditions.RightDown_ZoomPan_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDown, PointerModeState.Idle,
                PointerModeTransitionConditions.RightDown_Idle_Condition,
                PointerModeTransitionConditions.RightDown_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.Scroll, PointerModeState.Scroll,
                PointerModeTransitionConditions.Scroll_Scroll_Condition,
                PointerModeTransitionConditions.Scroll_Scroll_Action);

            stateMachine.AddTransition(
                PointerModeState.Scroll, PointerModeState.Idle,
                PointerModeTransitionConditions.Scroll_Idle_Condition,
                PointerModeTransitionConditions.Scroll_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.ZoomPan, PointerModeState.ZoomPan,
                PointerModeTransitionConditions.ZoomPan_ZoomPan_Condition,
                PointerModeTransitionConditions.ZoomPan_ZoomPan_Action);

            stateMachine.AddTransition(
                PointerModeState.ZoomPan, PointerModeState.Idle,
                PointerModeTransitionConditions.ZoomPan_Idle_Condition,
                PointerModeTransitionConditions.ZoomPan_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDoubleDown, PointerModeState.LeftDrag,
                PointerModeTransitionConditions.LeftDoubleDown_LeftDrag_Condition,
                PointerModeTransitionConditions.LeftDoubleDown_LeftDrag_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDrag, PointerModeState.LeftDrag,
                PointerModeTransitionConditions.LeftDrag_LeftDrag_Condition,
                PointerModeTransitionConditions.LeftDrag_LeftDrag_Action);

            stateMachine.AddTransition(
                PointerModeState.LeftDrag, PointerModeState.Idle,
                PointerModeTransitionConditions.LeftDrag_Idle_Condition,
                PointerModeTransitionConditions.LeftDrag_Idle_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDoubleDown, PointerModeState.RightDrag,
                PointerModeTransitionConditions.RightDoubleDown_RightDrag_Condition,
                PointerModeTransitionConditions.RightDoubleDown_RightDrag_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDrag, PointerModeState.RightDrag,
                PointerModeTransitionConditions.RightDrag_RightDrag_Condition,
                PointerModeTransitionConditions.RightDrag_RightDrag_Action);

            stateMachine.AddTransition(
                PointerModeState.RightDrag, PointerModeState.Idle,
                PointerModeTransitionConditions.RightDrag_Idle_Condition,
                PointerModeTransitionConditions.RightDrag_Idle_Action);
        }

    }
}
