using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using System.Collections.Generic;
using Windows.Foundation;


namespace RdClient.Shared.Test.Input.Mouse
{
    // The PointerModeZoomTests use  Mock.PointerManipulator instead of Mock.PointerManipulatorRecorder
     [TestClass]
    public class PointerModeZoomTests
    {        
        protected TestTimer _timer;
        private PointerEventDispatcher _consumer;
        private Mock.PointerManipulator _manipulator;

        [TestInitialize]
        public void TestSetup()
        {
            _timer = new TestTimer();
            _manipulator = new Mock.PointerManipulator();
            _consumer = new PointerEventDispatcher(_timer, _manipulator);
        }

        protected void ConsumeEvents(PointerEvent[] events)
        {
            foreach (PointerEvent e in events)
            {
                _consumer.ConsumeEvent(e);
            }
        }

        [TestMethod]
        public void PointerModel_ShouldVZoomIn()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 0.0, 20.0, 40.0, 20.0 }, null);

            ConsumeEvents(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(0, 40.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4),
                new PointerEvent(new Point(0.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(0.0, 30.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldHZoomOut()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 0.0, 20.0, 40.0 }, null);
            ConsumeEvents(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(30.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(40.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldRandomZoomIn()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 40.0, null, null }, null);
            ConsumeEvents(new PointerEvent[] { 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(40.0, 80.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4),
                new PointerEvent(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(30.0, 60.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldRandomZoomOut()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 40.0, null, null }, null);
            ConsumeEvents(new PointerEvent[] { 
                new PointerEvent(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(30.0, 60.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3),
                new PointerEvent(new Point(40.0, 80.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4)
            });
        }
    }
}
