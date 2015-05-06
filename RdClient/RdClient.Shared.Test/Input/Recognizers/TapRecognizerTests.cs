using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Input.Recognizers;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.Input.Recognizers
{
    [TestClass]
    public class TapRecognizerTests
    {
        [TestMethod]
        public void TestTap()
        {
            Mock.Timer timer = new Mock.Timer();
            TapRecognizer recognizer = new TapRecognizer(timer);

            bool tapped = false;

            recognizer.TapEvent += (s, e) =>
            {
                if(e.Type == TapEventType.Tap)
                {
                    tapped = true;
                }
                else
                {
                    Assert.Fail();
                }
            };

            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));
            timer.Expire();

            Assert.IsTrue(tapped);
        }

        [TestMethod]
        public void TestDoubleTap()
        {
            Mock.Timer timer = new Mock.Timer();
            TapRecognizer recognizer = new TapRecognizer(timer);

            bool doubleTapped = false;

            recognizer.TapEvent += (s, e) =>
            {
                if (e.Type == TapEventType.DoubleTap)
                {
                    doubleTapped = true;
                }
                else
                {
                    Assert.Fail();
                }
            };

            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));
            timer.Expire();

            Assert.IsTrue(doubleTapped);
        }

        [TestMethod]
        public void TestHolding()
        {
            Mock.Timer timer = new Mock.Timer();
            TapRecognizer recognizer = new TapRecognizer(timer);

            List<TapEventType> events = new List<TapEventType>();

            recognizer.TapEvent += (s, e) =>
            {
                events.Add(e.Type);
            };

            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            timer.Expire();
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(TapEventType.HoldingStarted, events[0]);
            Assert.AreEqual(TapEventType.HoldingCompleted, events[1]);
        }

        [TestMethod]
        public void TestHoldingCancelled()
        {
            Mock.Timer timer = new Mock.Timer();
            TapRecognizer recognizer = new TapRecognizer(timer);

            List<TapEventType> events = new List<TapEventType>();

            recognizer.TapEvent += (s, e) =>
            {
                events.Add(e.Type);
            };

            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            timer.Expire();
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerCancelled, new Point(0, 0)));

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(TapEventType.HoldingStarted, events[0]);
            Assert.AreEqual(TapEventType.HoldingCancelled, events[1]);
        }


        [TestMethod]
        public void TestTapHolding()
        {
            Mock.Timer timer = new Mock.Timer();
            TapRecognizer recognizer = new TapRecognizer(timer);

            List<TapEventType> events = new List<TapEventType>();

            recognizer.TapEvent += (s, e) =>
            {
                events.Add(e.Type);
            };

            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerPressed, new Point(0, 0)));
            timer.Expire();
            recognizer.Consume(new Mock.PointerEventBase(PointerEventAction.PointerReleased, new Point(0, 0)));

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(TapEventType.TapHoldingStarted, events[0]);
            Assert.AreEqual(TapEventType.HoldingCompleted, events[1]);
        }

    }
}
