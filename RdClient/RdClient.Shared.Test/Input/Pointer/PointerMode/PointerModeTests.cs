using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Input.Pointer.PointerMode;
using System.Collections.Generic;
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
            PointerEventOld pe1 = new PointerEventOld(new Point(0,0), false, new Point(0,0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsTrue(pmc.MoveThresholdExceeded(pe2));
            Assert.AreEqual(10 * GlobalConstants.MouseAcceleration, pmc.LastMoveDistance);
        }

        [TestMethod]
        public void PointerMode_MoveThresholdExceeded_DoubleTwoFingertap()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            List<PointerEventOld> pevents = new List<PointerEventOld>() {
                new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down),
                new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down),

                new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Up),
                new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Up),

                new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down),
                new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Down),

                new PointerEventOld(new Point(0, 5), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Update),
                
            };
            PointerEventOld pe = new PointerEventOld(new Point(0, 15), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 4, 0, TouchEventType.Update);

            foreach(PointerEventOld pevent in pevents)
            {
                pmc.TrackEvent(pevent);
            }

            pmc.MoveThresholdExceeded(pe);
        }

        [TestMethod]
        public void PointerMode_MoveThresholdExceeded_False()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.MoveThresholdExceeded(pe2));
        }

        [TestMethod]
        public void PointerMode_MoveThresholdExceeded_NewFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.MoveThresholdExceeded(pe2));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Up);
            PointerEventOld pe4 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);

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
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Up);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.AreEqual(0, pmc.NumberOfContacts(pe3));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts_LiftUntrackedFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Up);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.AreEqual(1, pmc.NumberOfContacts(pe3));
        }

        [TestMethod]
        public void PointerMode_NumberOfContacts_UpdateFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.AreEqual(1, pmc.NumberOfContacts(pe2));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 16), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsTrue(pmc.SpreadThresholdExceeded(pe3));
            Assert.AreEqual(6, pmc.LastSpreadDelta);
            Assert.AreEqual(new Point(0, 8), pmc.LastSpreadCenter);
            Assert.AreEqual(new Point(0, 3), pmc.LastPanDelta);
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_False()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 11), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe3));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_MoveFirstFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 5), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsTrue(pmc.SpreadThresholdExceeded(pe3));
        }

        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_ExtraFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 10), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down);
            PointerEventOld pe4 = new PointerEventOld(new Point(0, 5), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

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
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 5), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Update);

            pmc.TrackEvent(pe1);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe2));
        }


        [TestMethod]
        public void PointerMode_SpreadThresholdExceeded_UntrackedFinger()
        {
            Mock.Timer timer = new Mock.Timer();
            PointerContext pmc = new PointerContext(timer);
            PointerEventOld pe1 = new PointerEventOld(new Point(0, 0), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 1, 0, TouchEventType.Down);
            PointerEventOld pe2 = new PointerEventOld(new Point(0, 5), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 2, 0, TouchEventType.Down);
            PointerEventOld pe3 = new PointerEventOld(new Point(0, 15), false, new Point(0, 0), false, false, PointerTypeOld.Touch, 3, 0, TouchEventType.Down);

            pmc.TrackEvent(pe1);
            pmc.TrackEvent(pe2);

            Assert.IsFalse(pmc.SpreadThresholdExceeded(pe3));
        }
    }
}
