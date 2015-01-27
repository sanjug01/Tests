using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Mouse;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Mouse
{
     [TestClass]
    public class ClickModeTests : PointerInputTestsBase
    {

        [TestMethod]
        public void PointerModel_ShouldMoveAndClick()
        {
            this.ConsumptionMode = ConsumptionMode.DirectTouch;

            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3),
            });

            _timer.TriggerCallback();

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(10.0, 10.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(10.0, 10.0) }
            });
        }

        [TestMethod]
        public void PointerModel_ShouldMoveAndDoubleClick()
        {
            this.ConsumptionMode = ConsumptionMode.DirectTouch;

            ConsumeEventsHelper(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3),
                new PointerEvent(new Point(12.0, 12.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(12.0, 12.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3),
            });

            MouseAssertionHelper(new Mock.TestMousePointerEvent[] { 
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(12.0, 12.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(12.0, 12.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(12.0, 12.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.Move, Position = new Point(12.0, 12.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftPress, Position = new Point(12.0, 12.0) },
                new Mock.TestMousePointerEvent() { Type = MouseEventType.LeftRelease, Position = new Point(12.0, 12.0) }
            });
        }
    }
}
