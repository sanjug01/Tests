using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerModeTransitions
    {
        public static void AddTransitions(ref IStateMachine<PointerState, StateMachineEvent> stateMachine)
        {
            stateMachine.AddTransition(
                PointerState.Idle, PointerState.LeftDown, 
                PointerModeTransitionConditions.Idle_LeftDown_Condition, 
                PointerModeTransitionConditions.Idle_LeftDown_Action);

            stateMachine.AddTransition(
                PointerState.Idle, PointerState.LeftDoubleDown,
                PointerModeTransitionConditions.Idle_LeftDoubleDown_Condition,
                PointerModeTransitionConditions.Idle_LeftDoubleDown_Action);

            stateMachine.AddTransition(
                PointerState.LeftDoubleDown, PointerState.Idle,
                PointerModeTransitionConditions.LeftDoubleDown_Idle_Condition, 
                PointerModeTransitionConditions.LeftDoubleDown_Idle_Action);

            stateMachine.AddTransition(
                PointerState.LeftDown, PointerState.RightDown,
                PointerModeTransitionConditions.LeftDown_RightDown_Condition,
                PointerModeTransitionConditions.LeftDown_RightDown_Action);

            stateMachine.AddTransition(
                PointerState.LeftDown, PointerState.RightDoubleDown,
                PointerModeTransitionConditions.LeftDown_RightDoubleDown_Condition,
                PointerModeTransitionConditions.LeftDown_RightDoubleDown_Action);

            stateMachine.AddTransition(
                PointerState.LeftDown, PointerState.Move,
                PointerModeTransitionConditions.LeftDown_Move_Condition, 
                PointerModeTransitionConditions.LeftDown_Move_Action);

            stateMachine.AddTransition(
                PointerState.Move, PointerState.Move,
                PointerModeTransitionConditions.Move_Move_Condition,
                PointerModeTransitionConditions.Move_Move_Action);

            stateMachine.AddTransition(
                PointerState.Move, PointerState.Idle,
                PointerModeTransitionConditions.Move_Idle_Condition,
                PointerModeTransitionConditions.Move_Idle_Action);

            stateMachine.AddTransition(
                PointerState.LeftDown, PointerState.Idle,
                PointerModeTransitionConditions.LeftDown_Idle_Condition,
                PointerModeTransitionConditions.LeftDown_Idle_Action);

            stateMachine.AddTransition(
                PointerState.RightDown, PointerState.Scroll,
                PointerModeTransitionConditions.RightDown_Scroll_Condition,
                PointerModeTransitionConditions.RightDown_Scroll_Action);

            stateMachine.AddTransition(
                PointerState.RightDown, PointerState.ZoomPan,
                PointerModeTransitionConditions.RightDown_ZoomPan_Condition,
                PointerModeTransitionConditions.RightDown_ZoomPan_Action);

            stateMachine.AddTransition(
                PointerState.RightDown, PointerState.Idle,
                PointerModeTransitionConditions.RightDown_Idle_Condition,
                PointerModeTransitionConditions.RightDown_Idle_Action);

            stateMachine.AddTransition(
                PointerState.Scroll, PointerState.Scroll,
                PointerModeTransitionConditions.Scroll_Scroll_Condition,
                PointerModeTransitionConditions.Scroll_Scroll_Action);

            stateMachine.AddTransition(
                PointerState.Scroll, PointerState.Idle,
                PointerModeTransitionConditions.Scroll_Idle_Condition,
                PointerModeTransitionConditions.Scroll_Idle_Action);

            stateMachine.AddTransition(
                PointerState.ZoomPan, PointerState.ZoomPan,
                PointerModeTransitionConditions.ZoomPan_ZoomPan_Condition,
                PointerModeTransitionConditions.ZoomPan_ZoomPan_Action);

            stateMachine.AddTransition(
                PointerState.ZoomPan, PointerState.Idle,
                PointerModeTransitionConditions.ZoomPan_Idle_Condition,
                PointerModeTransitionConditions.ZoomPan_Idle_Action);

            stateMachine.AddTransition(
                PointerState.LeftDoubleDown, PointerState.LeftDrag,
                PointerModeTransitionConditions.LeftDoubleDown_LeftDrag_Condition,
                PointerModeTransitionConditions.LeftDoubleDown_LeftDrag_Action);

            stateMachine.AddTransition(
                PointerState.LeftDrag, PointerState.LeftDrag,
                PointerModeTransitionConditions.LeftDrag_LeftDrag_Condition,
                PointerModeTransitionConditions.LeftDrag_LeftDrag_Action);

            stateMachine.AddTransition(
                PointerState.LeftDrag, PointerState.Idle,
                PointerModeTransitionConditions.LeftDrag_Idle_Condition,
                PointerModeTransitionConditions.LeftDrag_Idle_Action);

            stateMachine.AddTransition(
                PointerState.RightDoubleDown, PointerState.RightDrag,
                PointerModeTransitionConditions.RightDoubleDown_RightDrag_Condition,
                PointerModeTransitionConditions.RightDoubleDown_RightDrag_Action);

            stateMachine.AddTransition(
                PointerState.RightDrag, PointerState.RightDrag,
                PointerModeTransitionConditions.RightDrag_RightDrag_Condition,
                PointerModeTransitionConditions.RightDrag_RightDrag_Action);

            stateMachine.AddTransition(
                PointerState.RightDrag, PointerState.Idle,
                PointerModeTransitionConditions.RightDrag_Idle_Condition,
                PointerModeTransitionConditions.RightDrag_Idle_Action);
        }

    }
}
