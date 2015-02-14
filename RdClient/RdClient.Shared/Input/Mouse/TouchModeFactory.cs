﻿using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Mouse
{
    public class TouchModeFactory
    {
        private static void AddDirectModeTransitions(ref IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine)
        {
            stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDown,
            (o) =>
            {
                return
                    o.Input.Inertia == false &&
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerState.Idle, PointerState.LeftDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 1 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimer.ClickTimerType.LeftClick) == false;
            },
            (o) => { o.Context.DoubleClickTimer.Stop(); });

            stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == true;
            },
            (o) => { });
            stateMachine.AddTransition(PointerState.LeftDown, PointerState.RightDoubleDown,
            (o) =>
            {
                return
                    o.Context.NumberOfContacts(o.Input) == 2 &&
                    o.Context.DoubleClickTimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick) == false;
            },
            (o) => { o.Context.DoubleClickTimer.Stop(); });
            stateMachine.AddTransition(PointerState.LeftDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) =>
            {
                if (o.Context.DoubleClickTimer.IsExpired(DoubleClickTimer.ClickTimerType.RightClick))
                    o.Context.DoubleClickTimer.Reset(DoubleClickTimer.ClickTimerType.LeftClick, o.Input);
            });

            stateMachine.AddTransition(PointerState.RightDown, PointerState.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { o.Context.DoubleClickTimer.Reset(DoubleClickTimer.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerState.RightDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.DoubleClickTimer.Reset(DoubleClickTimer.ClickTimerType.RightClick, o.Input); });
            stateMachine.AddTransition(PointerState.RightDown, PointerState.Scroll,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseScroll(o.Input); });

            stateMachine.AddTransition(PointerState.Scroll, PointerState.Scroll,
            (o) => { return 
                        o.Context.NumberOfContacts(o.Input) > 1 &&
                        o.Context.MoveThresholdExceeded(o.Input); 
            },
            (o) => { o.Context.MouseScroll(o.Input); });
            stateMachine.AddTransition(PointerState.Scroll, PointerState.LeftDown,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 1; },
            (o) => { });
            stateMachine.AddTransition(PointerState.Scroll, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.MouseLeftClick(o.Input); o.Context.MouseLeftClick(o.Input); });

            stateMachine.AddTransition(PointerState.LeftDoubleDown, PointerState.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.UpdateCursorPosition(o.Input); o.Context.PointerManipulator.SendMouseAction(MouseEventType.LeftPress); });

            stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerState.RightDoubleDown, PointerState.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.UpdateCursorPosition(o.Input); o.Context.PointerManipulator.SendMouseAction(MouseEventType.RightPress); });

            stateMachine.AddTransition(PointerState.LeftDrag, PointerState.LeftDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.LeftDrag, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.PointerManipulator.SendMouseAction(MouseEventType.LeftRelease); });

            stateMachine.AddTransition(PointerState.RightDrag, PointerState.RightDrag,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.RightDrag, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { o.Context.PointerManipulator.SendMouseAction(MouseEventType.RightRelease); });
        }

        private static void AddMoveTransitions(ref IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine)
        { 
            stateMachine.AddTransition(PointerState.Idle, PointerState.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.MouseMove(o.Input); });

            stateMachine.AddTransition(PointerState.LeftDown, PointerState.Move,
            (o) => { 
                return o.Context.MoveThresholdExceeded(o.Input); 
            },
            (o) => { 
                o.Context.MouseMove(o.Input); 
            });

            stateMachine.AddTransition(PointerState.Move, PointerState.Move,
            (o) => { return o.Context.MoveThresholdExceeded(o.Input); },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.Move, PointerState.Idle,
            (o) => { return o.Context.NumberOfContacts(o.Input) == 0; },
            (o) => { });

            stateMachine.AddTransition(PointerState.Inertia, PointerState.Inertia,
            (o) => { return o.Input.Inertia == true; },
            (o) => { o.Context.MouseMove(o.Input); });
            stateMachine.AddTransition(PointerState.Inertia, PointerState.Idle,
            (o) => { return o.Input.Inertia == false; },
            (o) => { });
        }

        private static void AddPinchAndZoomTransitions(ref IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine)
        {
            // TODO
        }


        public static IPointerEventConsumer CreatePointerMode(ITimer timer, IPointerManipulator manipulator)
        {
            IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine = new StateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>>();

            AddDirectModeTransitions(ref stateMachine);
            AddMoveTransitions(ref stateMachine);
            AddPinchAndZoomTransitions(ref stateMachine);

            stateMachine.SetStart(PointerState.Idle);

            return new TouchContext(timer, manipulator, stateMachine);
        }

        public static IPointerEventConsumer CreateDirectMode(ITimer timer, IPointerManipulator manipulator)
        {
            IStateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>> stateMachine = new StateMachine<PointerState, StateEvent<PointerEvent, ITouchContext>>();

            AddDirectModeTransitions(ref stateMachine);

            stateMachine.SetStart(PointerState.Idle);

            return new DirectTouchContext(timer, manipulator, stateMachine);
        }
    }
}
