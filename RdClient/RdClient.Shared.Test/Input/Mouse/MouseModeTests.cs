using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Input.Mouse;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Mouse
{
    public class MouseModeTests : PointerInputTestsBase
    {

        [TestMethod]
        public void PointerModel_Mouse_ShouldLeftDrag()
        {
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Mouse, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Mouse, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Mouse, 3)
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
            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, true, PointerType.Mouse, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, true, PointerType.Mouse, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Mouse, 3)
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() {Type = MouseEventType.RightPress, Position = new Point(0.0, 0.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() {Type = MouseEventType.RightRelease, Position = new Point(10.0, 10.0) }
            });
        }
    }
}
