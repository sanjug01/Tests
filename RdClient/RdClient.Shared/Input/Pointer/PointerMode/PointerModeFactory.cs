using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models; 

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeFactory
    {
        private static void AddDirectModeTransitions(ref IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDown,
            (o) =>
            {
                return
                    o.Input.Inertia == false &&
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.Timer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.Timer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == false;
            },
            (o) => { o.Context.Timer.Stop(); });

            stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == false;
            },
            (o) => { o.Context.Timer.Stop(); });
            stateMachine.AddTransition(PointerState.LeftDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) =>
            {
                if (o.Context.Timer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick))
                    o.Context.Timer.Reset(DoubleClickTimer.ClickTimerType.LeftClick, o.Input);
            });

            stateMachine.AddTransition(PointerState.RightDown, PointerState.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { o.Context.Timer.Reset(DoubleClickTimer.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerState.RightDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Timer.Reset(DoubleClickTimer.ClickTimerType.RightClick, o.Input); });

            stateMachine.AddTransition(PointerState.RightDown, PointerState.ZoomAndPan,
            (o) => { return o.Context.SpreadThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.ZoomAndPan(o.Input); });
            stateMachine.AddTransition(PointerState.ZoomAndPan, PointerState.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });    
            stateMachine.AddTransition(PointerState.ZoomAndPan, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { } );
            stateMachine.AddTransition(PointerState.ZoomAndPan, PointerState.ZoomAndPan,
            (o) => { return o.Context.SpreadThresholdExceeded(o.Input) || o.Context.MoveThresholdExceeded(o.Input, GlobalConstants.TouchZoomDeltaThreshold); },
            (o) => { o.Context.Control.ZoomAndPan(o.Input); });

            stateMachine.AddTransition(PointerState.RightDown, PointerState.Scroll,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input, GlobalConstants.TouchZoomDeltaThreshold); },
            (o) => { o.Context.Control.MouseScroll(o.Input); });
            stateMachine.AddTransition(PointerState.Scroll, PointerState.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });
            stateMachine.AddTransition(PointerState.Scroll, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });
            stateMachine.AddTransition(PointerState.Scroll, PointerState.Scroll,
            (o) =>
            {
                return
                   o.Context.NumberOfContacts(o.Input) > 1 &&
                   o.Context.MoveThresholdExceeded(o.Input);
            },
            (o) => { o.Context.Control.MouseScroll(o.Input); });

            stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.MouseLeftClick(o.Input); o.Context.Control.MouseLeftClick(o.Input); });

            stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.UpdateCursorPosition(o.Input); o.Context.Control.Manipulator.SendMouseAction(MouseEventType.LeftPress); });

            stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.UpdateCursorPosition(o.Input); o.Context.Control.Manipulator.SendMouseAction(MouseEventType.RightPress); });

            stateMachine.AddTransition(PointerState.LeftDrag, PointerState.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.LeftDrag, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.Manipulator.SendMouseAction(MouseEventType.LeftRelease); });

            stateMachine.AddTransition(PointerState.RightDrag, PointerState.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.RightDrag, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.Manipulator.SendMouseAction(MouseEventType.RightRelease); });
        }

        private static void AddMoveTransitions(ref IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerState.Idle, PointerState.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.Control.MouseMove(o.Input); });

            stateMachine.AddTransition(PointerState.LeftDown, PointerState.Move,
            (o) =>
            {
                return o.Context.MoveThresholdExceeded(o.Input);
            },
            (o) =>
            {
                o.Context.Control.MouseMove(o.Input);
            });

            stateMachine.AddTransition(PointerState.Move, PointerState.Move,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.Move, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerState.Inertia, PointerState.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.Inertia, PointerState.Idle,
            (o) => { return o.Input.Inertia == false; },
            (o) => { });
        }

        public static IPointerEventConsumer CreatePointerMode(ITimer timer, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> stateMachine = new StateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>>();

            AddDirectModeTransitions(ref stateMachine);
            AddMoveTransitions(ref stateMachine);

            stateMachine.SetStart(PointerState.Idle);

            PointerContext context = new PointerContext(timer);
            PointerModeControl control = new PointerModeControl(context, manipulator, panel);
            context.Control = control;

            return new PointerConsumer(context, stateMachine);
        }

        public static IPointerEventConsumer CreateDirectMode(ITimer timer, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            IStateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>> stateMachine = new StateMachine<PointerState, StateEvent<PointerEvent, IPointerContext>>();

            AddDirectModeTransitions(ref stateMachine);

            stateMachine.SetStart(PointerState.Idle);

            PointerContext context = new PointerContext(timer);
            DirectModeControl control = new DirectModeControl(context, manipulator, panel);
            context.Control = control;

            return new PointerConsumer(context, stateMachine);
        }
    }
}
