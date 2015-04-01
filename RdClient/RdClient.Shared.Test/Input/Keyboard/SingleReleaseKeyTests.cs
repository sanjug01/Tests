namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Input.Keyboard;
    using System.Collections.Generic;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class SingleReleaseKeyTests
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
                return new SingleReleaseKey(virtualKey, _sink, keyboardState);
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
        public void SingleReleaseKey_Press2KeysRelease1_AllKeysReleaaed()
        {
            const VirtualKey Key = VirtualKey.Shift;
            CorePhysicalKeyStatus
                statusLeft = new CorePhysicalKeyStatus() { ScanCode = 52 },
                statusRight = new CorePhysicalKeyStatus() { ScanCode = 48 };

            IList<TestSink.SinkEventArgs> reported = new List<TestSink.SinkEventArgs>();
            _sink.Keystroke += (sender, e) => reported.Add(e);

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, statusLeft);
            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, statusRight);
            _state.UpdateState(CoreAcceleratorKeyEventType.KeyUp, Key, statusLeft);

            Assert.AreEqual(4, reported.Count);

            Assert.AreEqual((int)statusLeft.ScanCode, reported[0].KeyCode);
            Assert.AreEqual(statusLeft.IsExtendedKey, reported[0].IsExtendedKey);
            Assert.IsFalse(reported[0].IsKeyReleased);

            Assert.AreEqual((int)statusRight.ScanCode, reported[1].KeyCode);
            Assert.AreEqual(statusRight.IsExtendedKey, reported[1].IsExtendedKey);
            Assert.IsFalse(reported[1].IsKeyReleased);

            Assert.AreEqual((int)statusRight.ScanCode, reported[2].KeyCode);
            Assert.AreEqual(statusRight.IsExtendedKey, reported[2].IsExtendedKey);
            Assert.IsTrue(reported[2].IsKeyReleased);

            Assert.AreEqual((int)statusLeft.ScanCode, reported[3].KeyCode);
            Assert.AreEqual(statusLeft.IsExtendedKey, reported[3].IsExtendedKey);
            Assert.IsTrue(reported[3].IsKeyReleased);

            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterPhysicalKey(statusLeft), typeof(DummyVirtualKey));
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterPhysicalKey(statusRight), typeof(DummyVirtualKey));
            Assert.IsInstanceOfType(((IKeyboardState)_state).UnregisterVirtualKey(Key), typeof(DummyVirtualKey));
        }
    }
}
