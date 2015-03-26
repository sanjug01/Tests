using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Mouse
{
    public class TestTimer : ITimer
    {
        System.Action _callback;

        public void TriggerCallback()
        {
            if (_callback != null)
            {
                _callback();
            }
        }

        public void Start(System.Action callback, System.TimeSpan period, bool recurring)
        {
            _callback = callback;
        }

        public void Stop()
        {

        }
    }

    [TestClass]
    public class PointerInputTestsBase
    {
        protected TestTimer _timer;
        private PointerEventDispatcher _consumer;
        private Mock.PointerManipulatorRecorder _manipulator;
        private Mock.RenderingPanel _panel;

        protected ConsumptionMode ConsumptionMode
        { 
            get { return _consumer.ConsumptionMode; }
            set { _consumer.ConsumptionMode = value; }
        }

        [TestInitialize]
        public void PointerModel_TestInitialize()
        {
            _timer = new TestTimer();
            _manipulator = new Mock.PointerManipulatorRecorder();
            _panel = new Mock.RenderingPanel();
            _consumer = new PointerEventDispatcher(_timer, _manipulator, _panel);
        }

        protected void ConsumeEventsHelper(PointerEvent[] events)
        {
            foreach (PointerEvent e in events)
            {
                _consumer.ConsumeEvent(e);
            }
        }

        protected void MouseAssertionHelper(Mock.TestMousePointerEvent[] expected)
        {
            Assert.AreEqual(expected.Length, _manipulator._mouseEventLog.Count);

            int i = 0;
            for (i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Type, _manipulator._mouseEventLog[i].Type);
                Assert.AreEqual(expected[i].Position, _manipulator._mouseEventLog[i].Position);
            }
        }
    }
}
