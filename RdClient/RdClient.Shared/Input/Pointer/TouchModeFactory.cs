using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Mouse
{
    public class TouchModeFactory
    {
        private static void AddDirectModeTransitions(ref IStateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.LeftDown,
            (o) =>
            {
                return
                    o.Input.Inertia == false &&
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimerOld.ClickTimerType.LeftClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.LeftDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimerOld.ClickTimerType.LeftClick) == false;
            },
            (o) => { o.Context.DoubleClickTimer.Stop(); });

            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.RightDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.RightDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick) == false;
            },
            (o) => { o.Context.DoubleClickTimer.Stop(); });
            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) =>
            {
                if (o.Context.DoubleClickTimer.IsExpired(DoubleClickTimerOld.ClickTimerType.RightClick))
                    o.Context.DoubleClickTimer.Reset(DoubleClickTimerOld.ClickTimerType.LeftClick, o.Input);
            });

            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { o.Context.DoubleClickTimer.Reset(DoubleClickTimerOld.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.DoubleClickTimer.Reset(DoubleClickTimerOld.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerStateOld.RightDown, PointerStateOld.Scroll,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseScroll(o.Input); });

            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.Scroll,
            (o) => { return 
                        o.Context.NumberOfContacts(o.Input) > 1 &&
                        o.Context.MoveThresholdExceeded(o.Input); 
            },
            (o) => { o.Context.MouseScroll(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });
            stateMachine.AddTransition(PointerStateOld.Scroll, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerStateOld.LeftDoubleDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.MouseLeftClick(o.Input); o.Context.MouseLeftClick(o.Input); });

            stateMachine.AddTransition(PointerStateOld.LeftDoubleDown, PointerStateOld.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.UpdateCursorPosition(o.Input); o.Context.PointerManipulator.SendMouseAction(MouseEventType.LeftPress); });

            stateMachine.AddTransition(PointerStateOld.RightDoubleDown, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerStateOld.RightDoubleDown, PointerStateOld.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.UpdateCursorPosition(o.Input); o.Context.PointerManipulator.SendMouseAction(MouseEventType.RightPress); });

            stateMachine.AddTransition(PointerStateOld.LeftDrag, PointerStateOld.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.LeftDrag, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.PointerManipulator.SendMouseAction(MouseEventType.LeftRelease); });

            stateMachine.AddTransition(PointerStateOld.RightDrag, PointerStateOld.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.RightDrag, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.PointerManipulator.SendMouseAction(MouseEventType.RightRelease); });
        }

        private static void AddMoveTransitions(ref IStateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>> stateMachine)
        { 
            stateMachine.AddTransition(PointerStateOld.Idle, PointerStateOld.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.MouseMove(o.Input); });

            stateMachine.AddTransition(PointerStateOld.LeftDown, PointerStateOld.Move,
            (o) => { 
                return o.Context.MoveThresholdExceeded(o.Input); 
            },
            (o) => { 
                o.Context.MouseMove(o.Input); 
            });

            stateMachine.AddTransition(PointerStateOld.Move, PointerStateOld.Move,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Move, PointerStateOld.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerStateOld.Inertia, PointerStateOld.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerStateOld.Inertia, PointerStateOld.Idle,
            (o) => { return o.Input.Inertia == false; },
            (o) => { });
        }

        public static IPointerEventConsumerOld CreatePointerMode(ITimer timer, IPointerManipulator manipulator)
        {
            IStateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>> stateMachine = new StateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>>();

            AddDirectModeTransitions(ref stateMachine);
            AddMoveTransitions(ref stateMachine);

            stateMachine.SetStart(PointerStateOld.Idle);

            return new TouchContext(timer, manipulator, stateMachine);
        }

        public static IPointerEventConsumerOld CreateDirectMode(ITimer timer, IPointerManipulator manipulator)
        {
            IStateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>> stateMachine = new StateMachine<PointerStateOld, StateEvent<PointerEventOld, ITouchContext>>();

            AddDirectModeTransitions(ref stateMachine);

            stateMachine.SetStart(PointerStateOld.Idle);

            return new DirectTouchContext(timer, manipulator, stateMachine);
        }
    }
}
