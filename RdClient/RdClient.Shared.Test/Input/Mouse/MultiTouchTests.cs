using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Mouse
{
    [TestClass]
    public class MultiTouchTests
    {
        [TestMethod]
        public void MultiTouchTests_SkipDuplicate()
        {
            using(Mock.PointerManipulator manipulator = new Mock.PointerManipulator())
            {
                MultiTouchMode mtm = new MultiTouchMode(manipulator);
                PointerEventOld pe1 = new PointerEventOld(
                    new Point(10, 10), false, new Point(0, 0), false, false,
                    PointerTypeOld.Touch, 23, 1234, TouchEventType.Update);
                PointerEventOld pe2 = new PointerEventOld(
                    new Point(10, 10), false, new Point(0, 0), false, false,
                    PointerTypeOld.Touch, 23, 1234, TouchEventType.Update);

                manipulator.Expect("SendTouchAction", 
                    new List<object> { TouchEventType.Update, 23, new Point(10,10), new Point(0,0) }, null);

                mtm.ConsumptionMode = ConsumptionMode.MultiTouch;
                mtm.ConsumeEvent(pe1);
                mtm.ConsumeEvent(pe2);
            }
        }
    }
}
