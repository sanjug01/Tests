using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Mouse;
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

    public class TestMousePointerEvent
    {
        public MouseEventType Type { get; set; }
        public Point Position { get; set; }
    }

    public class TestPointerManipulator : IPointerManipulator
    {
        public List<TestMousePointerEvent> _eventLog = new List<TestMousePointerEvent>();

        public double MouseAcceleration
        {
            get { return 1.0; }
            set { }
        }

        public Point MousePosition
        {
            get;
            set;
        }

        public void SendMouseAction(MouseEventType type)
        {
            _eventLog.Add(new TestMousePointerEvent() { Position = MousePosition, Type = type });
        }
    }

    [TestClass]
    public class PointerInputTestsBase
    {
        protected TestTimer _timer;
        private PointerEventDispatcher _consumer;
        private TestPointerManipulator _manipulator;

        protected bool ClickModeEnabled { get { return _consumer.DirectModeEnabled; } set { _consumer.DirectModeEnabled = value; } }

        [TestInitialize]
        public void PointerModel_TestInitialize()
        {
            _timer = new TestTimer();
            _manipulator = new TestPointerManipulator();
            _consumer = new PointerEventDispatcher(_timer, _manipulator);
        }

        protected void ConsumeEventsHelper(PointerEvent[] events)
        {
            foreach (PointerEvent e in events)
            {
                _consumer.ConsumeEvent(e);
            }
        }

        protected void AssertionHelper(TestMousePointerEvent[] expected)
        {
            Assert.AreEqual(expected.Length, _manipulator._eventLog.Count);

            int i = 0;
            for (i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Type, _manipulator._eventLog[i].Type);
                Assert.AreEqual(expected[i].Position, _manipulator._eventLog[i].Position);
            }
        }
    }
}
