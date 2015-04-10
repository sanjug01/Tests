using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models; 

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeFactory
    {
        private static void AddDirectModeTransitions(ref IStateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.LeftDown,
            (o) =>
            {
                return
                    o.Input.Inertia == false &&
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.Timer.IsExpired(DoubleClickTimerOld.ClickTimerType.LeftClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.LeftDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.Timer.IsExpired(DoubleClickTimerOld.ClickTimerType.LeftClick) == false;
            },
            (o) => { o.Context.Timer.Stop(); });

            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.RightDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.Timer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.RightDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.Timer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick) == false;
            },
            (o) => { o.Context.Timer.Stop(); });
            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) =>
            {
                if (o.Context.Timer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick))
                    o.Context.Timer.Reset(DoubleClickTimerOld.ClickTimerType.LeftClick, o.Input);
            });

            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { o.Context.Timer.Reset(DoubleClickTimerOld.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Timer.Reset(DoubleClickTimerOld.ClickTimerType.RightClick, o.Input); });

            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.ZoomAndPan,
            (o) => { return o.Context.SpreadThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.ZoomAndPan(o.Input); });
            stateMachine.AddTransition(PointerStateOld.ZoomAndPan, PointerStateOld.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });    
            stateMachine.AddTransition(PointerStateOld.ZoomAndPan, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { } );
            stateMachine.AddTransition(PointerStateOld.ZoomAndPan, PointerStateOld.ZoomAndPan,
            (o) => { return o.Context.SpreadThresholdExceeded(o.Input) || o.Context.MoveThresholdExceeded(o.Input, GlobalConstants.TouchZoomDeltaThreshold); },
            (o) => { o.Context.Control.ZoomAndPan(o.Input); });

            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.Scroll,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input, GlobalConstants.TouchZoomDeltaThreshold); },
            (o) => { o.Context.Control.MouseScroll(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.Scroll,
            (o) =>
            {
                return
                   o.Context.NumberOfContacts(o.Input) > 1 &&
                   o.Context.MoveThresholdExceeded(o.Input);
            },
            (o) => { o.Context.Control.MouseScroll(o.Input); });

            stateMachine.AddTransition(PointerStateOld.LeftDoubleDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.MouseLeftClick(o.Input); o.Context.Control.MouseLeftClick(o.Input); });

            stateMachine.AddTransition(PointerStateOld.LeftDoubleDown, PointerStateOld.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.UpdateCursorPosition(o.Input); o.Context.Control.Manipulator.SendMouseAction(MouseEventType.LeftPress); });

            stateMachine.AddTransition(PointerStateOld.RightDoubleDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerStateOld.RightDoubleDown, PointerStateOld.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.UpdateCursorPosition(o.Input); o.Context.Control.Manipulator.SendMouseAction(MouseEventType.RightPress); });

            stateMachine.AddTransition(PointerStateOld.LeftDrag, PointerStateOld.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.LeftDrag, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.Manipulator.SendMouseAction(MouseEventType.LeftRelease); });

            stateMachine.AddTransition(PointerStateOld.RightDrag, PointerStateOld.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.RightDrag, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.Control.Manipulator.SendMouseAction(MouseEventType.RightRelease); });
        }

        private static void AddMoveTransitions(ref IStateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.Control.MouseMove(o.Input); });

            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.Move,
            (o) =>
            {
                return o.Context.MoveThresholdExceeded(o.Input);
            },
            (o) =>
            {
                o.Context.Control.MouseMove(o.Input);
            });

            stateMachine.AddTransition(PointerStateOld.Move, PointerStateOld.Move,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Move, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerStateOld.Inertia, PointerStateOld.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.Control.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Inertia, PointerStateOld.Idle,
            (o) => { return o.Input.Inertia == false; },
            (o) => { });
        }

        public static IPointerEventConsumer CreatePointerMode(ITimer timer, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            IStateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>> stateMachine = new StateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>>();

            AddDirectModeTransitions(ref stateMachine);
            AddMoveTransitions(ref stateMachine);

            stateMachine.SetStart(PointerStateOld.Idle);

            PointerContext context = new PointerContext(timer);
            PointerModeControl control = new PointerModeControl(context, manipulator, panel);
            context.Control = control;

            return new PointerConsumer(context, stateMachine);
        }

        public static IPointerEventConsumer CreateDirectMode(ITimer timer, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            IStateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>> stateMachine = new StateMachine<PointerStateOld, StateEvent<PointerEventOld, IPointerContext>>();

            AddDirectModeTransitions(ref stateMachine);

            stateMachine.SetStart(PointerStateOld.Idle);

            PointerContext context = new PointerContext(timer);
            DirectModeControl control = new DirectModeControl(context, manipulator, panel);
            context.Control = control;

            return new PointerConsumer(context, stateMachine);
        }
    }
}
