using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Input.Pointer;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Mouse
{
    [TestClass]
    public class MouseModeTests : PointerInputTestsBase
    {

        [TestMethod]
        public void PointerModel_Mouse_ShouldLeftDrag()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Mouse, 3),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Mouse, 3),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Mouse, 3)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() {Type = MouseEventType.LeftPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.LeftRelease, Position = new Point(10.0, 10.0) }
            });
        }

        [TestMethod]
        public void PointerModel_Mouse_ShouldRightDrag()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, true, PointerTypeOld.Mouse, 3),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, true, PointerTypeOld.Mouse, 3),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Mouse, 3)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() {Type = MouseEventType.RightPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.RightRelease, Position = new Point(10.0, 10.0) }
            });
        }

        [TestMethod]
        public void PointerModel_Mouse_ShouldScrollVertical()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Mouse, 3, 0, TouchEventType.Unknown, 30, false),
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() {Type = MouseEventType.MouseWheel, Position = new Point(0.0, 30.0) },
            });
        }

        [TestMethod]
        public void PointerModel_Mouse_ShouldScrollHorizontal()
        {
            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Mouse, 3, 0, TouchEventType.Unknown, 30, true),
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() {Type = MouseEventType.MouseHWheel, Position = new Point(30.0, 0.0) },
            });
        }
    }
}
