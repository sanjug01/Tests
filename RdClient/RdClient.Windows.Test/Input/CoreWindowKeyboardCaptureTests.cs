namespace RdClient.Windows.Test.Input
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Input;
    using RdClient.Shared.Input;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class CoreWindowKeyboardCaptureTests
    {
        [TestMethod]
        public void Subscribe_ReportKeystroke_KeystrokeEmitted()
        {
            IList<Tuple<object, KeystrokeEventArgs>> events = new List<Tuple<object, KeystrokeEventArgs>>();
            CoreWindowKeyboardCapture cwCapture = new CoreWindowKeyboardCapture();
            IKeyboardCapture capture = cwCapture;
            IKeyboardCaptureSink sink = cwCapture;

            capture.Keystroke += (s, e) => events.Add(new Tuple<object, KeystrokeEventArgs>(s, e));
            //
            // Call the interface called by the Core object directly.
            //
            sink.ReportKeystroke(32, false, false, false);
            Assert.AreEqual(1, events.Count);
            Assert.AreSame(cwCapture, events[0].Item1);
            Assert.AreEqual(32, events[0].Item2.KeyCode);
            Assert.AreEqual(false, events[0].Item2.IsScanCode);
            Assert.AreEqual(false, events[0].Item2.IsExtendedKey);
            Assert.AreEqual(false, events[0].Item2.IsKeyReleased);
        }

        [TestMethod]
        public void Unsubscribe_ReportKeystroke_KeystrokeNotEmitted()
        {
            IList<Tuple<object, KeystrokeEventArgs>> events = new List<Tuple<object, KeystrokeEventArgs>>();
            CoreWindowKeyboardCapture cwCapture = new CoreWindowKeyboardCapture();
            IKeyboardCapture capture = cwCapture;
            IKeyboardCaptureSink sink = cwCapture;
            EventHandler<KeystrokeEventArgs> handler = (s, e) => events.Add(new Tuple<object, KeystrokeEventArgs>(s, e));

            capture.Keystroke += handler;
            //
            // Call the interface called by the Core object directly.
            //
            sink.ReportKeystroke(32, false, false, false);
            capture.Keystroke -= handler;
            sink.ReportKeystroke(33, false, false, false);
            Assert.AreEqual(1, events.Count);
            Assert.AreSame(cwCapture, events[0].Item1);
            Assert.AreEqual(32, events[0].Item2.KeyCode);
            Assert.AreEqual(false, events[0].Item2.IsScanCode);
            Assert.AreEqual(false, events[0].Item2.IsExtendedKey);
            Assert.AreEqual(false, events[0].Item2.IsKeyReleased);
        }
    }
}
