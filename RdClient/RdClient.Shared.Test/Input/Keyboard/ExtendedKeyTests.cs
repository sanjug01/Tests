namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Collections.Generic;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class ExtendedKeyTests
    {
        private TestSink _sink;
        private KeyboardState _state;

        private sealed class Factory : IVirtualKeyFactory
        {
            private readonly IKeyboardCaptureSink _sink;

            public Factory(IKeyboardCaptureSink sink)
            {
                _sink = sink;
            }

            IVirtualKey IVirtualKeyFactory.MakeVirtualKey(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus, IKeyboardState keyboardState)
            {
                return new ExtendedKey(virtualKey, _sink, keyboardState);
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _sink = new TestSink();
            _state = new KeyboardState(_sink, new Factory(_sink));
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _sink = null;
            _state = null;
        }

        [TestMethod]
        public void ExtendedKey_DownUp_UpDownReported()
        {
            const VirtualKey Key = VirtualKey.Enter;
            IList<TestSink.SinkEventArgs> reported = new List<TestSink.SinkEventArgs>();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _sink.Keystroke += (sender, e) =>
            {
                reported.Add(e);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            status.IsKeyReleased = true;
            _state.UpdateState(CoreAcceleratorKeyEventType.KeyUp, Key, status);

            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual((int)status.ScanCode, reported[0].KeyCode);
            Assert.IsTrue(reported[0].IsScanCode);
            Assert.IsFalse(reported[0].IsKeyReleased);

            Assert.AreEqual((int)status.ScanCode, reported[1].KeyCode);
            Assert.IsTrue(reported[1].IsScanCode);
            Assert.IsTrue(reported[1].IsKeyReleased);
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterPhysicalKey(status), typeof(DummyVirtualKey));
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterVirtualKey(Key), typeof(DummyVirtualKey));
        }

        [TestMethod]
        public void ExtendedKey_DownDownUp_DownDownUpReported()
        {
            const VirtualKey Key = VirtualKey.Enter;
            IList<TestSink.SinkEventArgs> reported = new List<TestSink.SinkEventArgs>();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _sink.Keystroke += (sender, e) =>
            {
                reported.Add(e);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            status.IsKeyReleased = true;
            _state.UpdateState(CoreAcceleratorKeyEventType.KeyUp, Key, status);

            Assert.AreEqual(3, reported.Count);
            Assert.AreEqual((int)status.ScanCode, reported[0].KeyCode);
            Assert.IsTrue(reported[0].IsScanCode);
            Assert.IsFalse(reported[0].IsKeyReleased);
            Assert.AreEqual((int)status.ScanCode, reported[1].KeyCode);
            Assert.IsTrue(reported[1].IsScanCode);
            Assert.IsFalse(reported[1].IsKeyReleased);

            Assert.AreEqual((int)status.ScanCode, reported[2].KeyCode);
            Assert.IsTrue(reported[2].IsScanCode);
            Assert.IsTrue(reported[2].IsKeyReleased);
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterPhysicalKey(status), typeof(DummyVirtualKey));
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterVirtualKey(Key), typeof(DummyVirtualKey));
        }

        [TestMethod]
        public void ExtendedKey_Up_NothingReported()
        {
            const VirtualKey Key = (VirtualKey)231;
            IList<TestSink.SinkEventArgs> reported = new List<TestSink.SinkEventArgs>();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 0, IsKeyReleased = true };

            _sink.Keystroke += (sender, e) =>
            {
                reported.Add(e);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyUp, Key, status);

            Assert.AreEqual(0, reported.Count);
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterPhysicalKey(status), typeof(DummyVirtualKey));
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterVirtualKey(Key), typeof(DummyVirtualKey));
        }
    }
}
