using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Input.Pointer.PointerMode;
using Windows.Foundation;


namespace RdClient.Shared.Test.Input.Pointer.PointerMode
{
    [TestClass]
    public class PointerModeTests
    {
        [TestMethod]
        public void PointerMode_MoveThresholdExceeded()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0,0), false, new Point(0,0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsTrue(pmc.MoveThresholdExceeded(pe2));
            Assert.AreEqual(10 * GlobalConstants.MouseAcceleration, pmc.LastMoveDistance);
        }

        [TestMethod]
        public void PointerMode_MoveThresholdExceeded_False()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.MoveThresholdExceeded(pe2));
        }

        [TestMethod]
        public void PointerMode_MoveThresholdExceeded_NewFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.MoveThresholdExceeded(pe2));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);
            PointerEvent pe3 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Up);
            PointerEvent pe4 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);
            pmc.TrackEvent(pe3);

            Assert.AreEqual(1, pmc.NumberOfContacts(pe4));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts_LiftFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);
            PointerEvent pe3 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Up);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.AreEqual(0, pmc.NumberOfContacts(pe3));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts_LiftUntrackedFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);
            PointerEvent pe3 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Up);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.AreEqual(1, pmc.NumberOfContacts(pe3));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts_UpdateFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.AreEqual(1, pmc.NumberOfContacts(pe2));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);
            PointerEvent pe3 = new PointerEvent(new Point(0, 16), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsTrue(pmc.SpreadThresholdExceeded(pe3));
            Assert.AreEqual(6, pmc.LastSpreadDelta);
            Assert.AreEqual(new Point(0, 8), pmc.LastSpreadCenter);
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_False()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);
            PointerEvent pe3 = new PointerEvent(new Point(0, 11), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe3));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_MoveFirstFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);
            PointerEvent pe3 = new PointerEvent(new Point(0, 5), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsTrue(pmc.SpreadThresholdExceeded(pe3));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_ExtraFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);
            PointerEvent pe3 = new PointerEvent(new Point(0, 10), false, new Point(0, 0), false, false, PointerType.Touch, 3, 0, TouchEventType.Down);
            PointerEvent pe4 = new PointerEvent(new Point(0, 5), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);
            pmc.TrackEvent(pe3);

            Assert.IsTrue(pmc.SpreadThresholdExceeded(pe4));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_NotEnoughFingers()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 5), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe2));
        }


        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_UntrackedFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEvent pe1 = new PointerEvent(new Point(0, 0), false, new Point(0, 0), false, false, PointerType.Touch, 1, 0, TouchEventType.Down);
            PointerEvent pe2 = new PointerEvent(new Point(0, 5), false, new Point(0, 0), false, false, PointerType.Touch, 2, 0, TouchEventType.Down);
            PointerEvent pe3 = new PointerEvent(new Point(0, 15), false, new Point(0, 0), false, false, PointerType.Touch, 3, 0, TouchEventType.Down);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe3));
        }
    }
}
