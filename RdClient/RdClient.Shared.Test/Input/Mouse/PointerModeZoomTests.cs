using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
        private PointerEventDispatcherOld _consumer;
        private Mock.PointerManipulator _manipulator;
        private Mock.RenderingPanel _panel;

        [TestInitialize]
        public void TestSetup()
        {
            _timer = new TestTimer();
            _manipulator = new Mock.PointerManipulator();
            _panel = new Mock.RenderingPanel();
            _consumer = new PointerEventDispatcherOld(_timer, _manipulator, _panel);
        }

        protected void ConsumeEvents(PointerEventOld[] events)
        {
            foreach (PointerEventOld e in events)
            {
                _consumer.ConsumeEvent(e);
            }
        }

        [TestMethod]
        public void PointerModel_ShouldVZoomIn()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 0.0, 20.0, 40.0, 20.0 }, null);

            ConsumeEvents(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(0, 40.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4),
                new PointerEventOld(new Point(0.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(0.0, 30.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldHZoomOut()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 0.0, 20.0, 40.0 }, null);
            ConsumeEvents(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(30.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(40.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldRandomZoomIn()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 40.0, null, null }, null);
            ConsumeEvents(new PointerEventOld[] { 
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(40.0, 80.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4),
                new PointerEventOld(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(30.0, 60.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4)
            });
        }

        [TestMethod]
        public void PointerModel_ShouldRandomZoomOut()
        {
            _manipulator.Expect("SendPinchAndZoom", new List<object> { 20.0, 40.0, null, null }, null);
            ConsumeEvents(new PointerEventOld[] { 
                new PointerEventOld(new Point(10.0, 20.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(30.0, 60.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4),
                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 3),
                new PointerEventOld(new Point(40.0, 80.0), false, new Point(0.0, 0.0), true, false, PointerTypeOld.Touch, 4)
            });
        }
    }
}
