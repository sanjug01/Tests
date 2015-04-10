using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
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

            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
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

            ConsumeEventsHelper(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
                new PointerEventOld(new Point(12.0, 12.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(12.0, 12.0), false, new Point(0.0, 0.0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Up),
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
