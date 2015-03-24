using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using System.Collections.Generic;
using Windows.Foundation;


namespace RdClient.Shared.Test.Input.Mouse
{
     [TestClass]
    public class PointerModeTests : PointerInputTestsBase
    {

        [TestMethod]
        public void PointerModel_ShouldMove()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(20.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(20.0, 20.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldMoveOnce()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldMoveWithInertia()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                // finger down
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),

                // move finger
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),

                // lift finger
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up),

                // inertia
                new PointerEvent(new Point(0.0, 0.0), true, new Point(10.0, 10.0), false, false, PointerType.Touch, 3),

                // inertia
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(20.0, 20.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldLeftClick()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up)
            });

            _timer.TriggerCallback();

            // we didn't send any pointer move gestures so the click event should be at the current cursor position which is 0.0
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(0.0, 0.0) }
            });
        }


        [TestMethod]
        public void PointerModel_ShouldDoubleLeftClick()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up)
            });

            // we didn't send any pointer move gestures so the double click event should be at the current cursor position which is 0.0
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(0.0, 0.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldLeftDrag()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(1.0, 1.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(10.0, 10.00) }
            });            
        }

        [TestMethod]
        public void PointerModel_ShouldRightDrag()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                // right tap 1
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Down),

                // right tap 2
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 4, 0, TouchEventType.Up),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Down),

                // drag
                new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Update),

                // drag
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Update),

                // release
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3, 0, TouchEventType.Up),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 4, 0, TouchEventType.Up)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.RightPress, Position = new Point(1.0, 1.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.RightRelease, Position = new Point(10.0, 10.00) }
            });  
        }

        [TestMethod]
        public void PointerModel_ShouldScroll()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Down),
                new PointerEvent(new Point(0.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Update)
            });

            // there is a x5 multiplier in the touch context to make the scrolling more immediate on surface tablets
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.MouseWheel, Position = new Point(0.0, 50.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldHScroll()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Down),
                new PointerEvent(new Point(10.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3, 0, TouchEventType.Update),
                new PointerEvent(new Point(20.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4, 0, TouchEventType.Update)
            });

            // there is a x5 multiplier in the touch context to make the scrolling more immediate on surface tablets
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.MouseHWheel, Position = new Point(50.0, 0.0) }
            });
        }
    }
}
