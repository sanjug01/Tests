using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(20.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(20.0, 20.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldNotMove()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update),
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
            });
        }

        [TestMethod]
        public void PointerModel_ShouldMoveOnce()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldMoveWithInertia()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                // finger down
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),

                // move finger
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),

                // lift finger
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),

                // inertia
                new PointerEventOld(new Point(0.0, 0.0), true, new Point(10.0, 10.0), false, false, PointerTypeOld.Touch, 3),

                // inertia
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(20.0, 20.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldLeftClick()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up)
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
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up)
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
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up)
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
            ConsumeEventsHelper(new PointerEventOld[] { 
                // right tap 1
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Down),

                // right tap 2
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Up),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Down),

                // drag
                new PointerEventOld(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update),

                // drag
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update),

                // release
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Up)
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
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Down),
                new PointerEventOld(new Point(0.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update)
            });

            // there is a x5 multiplier in the touch context to make the scrolling more immediate on surface tablets
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.MouseWheel, Position = new Point(0.0, 50.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldHScroll()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                new PointerEventOld(new Point(20.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update)
            });

            // there is a x5 multiplier in the touch context to make the scrolling more immediate on surface tablets
            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.MouseHWheel, Position = new Point(50.0, 0.0) }
            });
        }
    }
}
