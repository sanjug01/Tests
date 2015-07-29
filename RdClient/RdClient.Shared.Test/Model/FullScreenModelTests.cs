using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class FullScreenModelTests
    {
        // Happy case test for entering full screen.
        /* 
            the expected order of things:
            
            1. consumer calls EnterFullSCreen()
            2. EnteringFullScreen event is fired by the Model to let us know we have started
            3. platform starts firing WindowSizeChanged events on the CoreWindow
            4. We receive an IsFullScreenChanged event to signify that the corresponding platform boolean has changed
            5. the timer only expires after there haven't been any WindowSizeChanged events for a while
            6. once the timer has expired, EnteredFullScreen is fired by the Model to let us know we are now in Full Screen Mode
        */
        [TestMethod]
        public void Test_FullScreen()
        {
            FullScreenModel fsm = new FullScreenModel();
            using (Mock.FullScreen fs = new Mock.FullScreen())
            using (Mock.TimerFactory tf = new Mock.TimerFactory())
            using (Mock.WindowSize ws = new Mock.WindowSize())
            {
                List<int> eOrder = new List<int>();

                Mock.Timer timer = new Mock.Timer();
                tf.Expect("CreateTimer", new List<object> { }, timer);

                fsm.FullScreen = fs;
                fsm.TimerFactory = tf;
                fsm.WindowSize = ws;

                fs.Expect("EnterFullScreen", new List<object> { }, null);

                IFullScreenModel ifsm = fsm;

                ifsm.EnteringFullScreen += (s, o) => eOrder.Add(0);
                ifsm.FullScreenChange += (s, o) => eOrder.Add(1);
                ifsm.EnteredFullScreen += (s, o) => eOrder.Add(2);

                ifsm.EnterFullScreen();
                ws.EmitSizeChanged(new Size(10, 10));
                fs.EmitIsFullScreenModeChanged();
                timer.Expire();

                Assert.AreEqual(0, eOrder[0]);
                Assert.AreEqual(1, eOrder[1]);
                Assert.AreEqual(2, eOrder[2]);
                Assert.AreEqual(3, eOrder.Count);
            }
        }

        // same as above but verifies that even when we receive many WindowSizeChanged events
        // we only fire each of the output events once
        [TestMethod]
        public void Test_FullScreen_Debounce()
        {
            FullScreenModel fsm = new FullScreenModel();
            using (Mock.FullScreen fs = new Mock.FullScreen())
            using (Mock.TimerFactory tf = new Mock.TimerFactory())
            using (Mock.WindowSize ws = new Mock.WindowSize())
            {
                List<int> eOrder = new List<int>();

                Mock.Timer timer = new Mock.Timer();
                tf.Expect("CreateTimer", new List<object> { }, timer);

                fsm.FullScreen = fs;
                fsm.TimerFactory = tf;
                fsm.WindowSize = ws;

                fs.Expect("EnterFullScreen", new List<object> { }, null);

                IFullScreenModel ifsm = fsm;

                ifsm.EnteringFullScreen += (s, o) => eOrder.Add(0);
                ifsm.FullScreenChange += (s, o) => eOrder.Add(1);
                ifsm.EnteredFullScreen += (s, o) => eOrder.Add(2);

                ifsm.EnterFullScreen();
                ws.EmitSizeChanged(new Size(10, 10));
                fs.EmitIsFullScreenModeChanged();
                ws.EmitSizeChanged(new Size(10, 10));
                ws.EmitSizeChanged(new Size(10, 10));
                ws.EmitSizeChanged(new Size(10, 10));
                timer.Expire();


                Assert.AreEqual(0, eOrder[0]);
                Assert.AreEqual(1, eOrder[1]);
                Assert.AreEqual(2, eOrder[2]);
                Assert.AreEqual(3, eOrder.Count);
            }
        }

        // same as above but verifies that even when we receive don't receive any WindowSizeChanged events
        // we still receive an EnteredFullScreen event
        [TestMethod]
        public void Test_FullScreen_AlreadyFullScreen()
        {
            FullScreenModel fsm = new FullScreenModel();
            using (Mock.FullScreen fs = new Mock.FullScreen())
            using (Mock.TimerFactory tf = new Mock.TimerFactory())
            using (Mock.WindowSize ws = new Mock.WindowSize())
            {
                List<int> eOrder = new List<int>();

                Mock.Timer timer = new Mock.Timer();
                tf.Expect("CreateTimer", new List<object> { }, timer);

                fsm.FullScreen = fs;
                fsm.TimerFactory = tf;
                fsm.WindowSize = ws;

                fs.Expect("EnterFullScreen", new List<object> { }, null);

                IFullScreenModel ifsm = fsm;

                ifsm.EnteringFullScreen += (s, o) => eOrder.Add(0);
                ifsm.EnteredFullScreen += (s, o) => eOrder.Add(1);

                ifsm.EnterFullScreen();
                timer.Expire();

                Assert.AreEqual(0, eOrder[0]);
                Assert.AreEqual(1, eOrder[1]);
                Assert.AreEqual(2, eOrder.Count);
            }
        }

        [TestMethod]
        public void Test_WindowDeactivate()
        {
            FullScreenModel fsm = new FullScreenModel();
            using (Mock.FullScreen fs = new Mock.FullScreen())
            using (Mock.TimerFactory tf = new Mock.TimerFactory())
            using (Mock.WindowSize ws = new Mock.WindowSize())
            {
                List<int> eOrder = new List<int>();

                Mock.Timer timer = new Mock.Timer();
                tf.Expect("CreateTimer", new List<object> { }, timer);

                fsm.FullScreen = fs;
                fsm.TimerFactory = tf;
                fsm.WindowSize = ws;

                fs.Expect("EnterFullScreen", new List<object> { }, null);

                IFullScreenModel ifsm = fsm;

                ifsm.EnteringFullScreen += (s, o) => eOrder.Add(0);
                ifsm.FullScreenChange += (s, o) => eOrder.Add(1);
                ifsm.EnteredFullScreen += (s, o) => eOrder.Add(2);

                ifsm.EnterFullScreen();
                ws.EmitSizeChanged(new Size(10, 10));
                fs.EmitIsFullScreenModeChanged();
                ws.EmitActivated(WindowActivation.Deactivated);
                timer.Expire();

                Assert.AreEqual(0, eOrder[0]);
                Assert.AreEqual(1, eOrder[1]);
                Assert.AreEqual(2, eOrder[2]);
                Assert.AreEqual(3, eOrder.Count);
            }
        }
    }
}
