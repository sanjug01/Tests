namespace RdClient.Shared.Test.Input
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Input;
    using System.Collections.Generic;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class CoreWindowKeyboardCoreTests
    {
        private struct Keystroke
        {
            public int keyCode;
            public bool isScanCode, isExtendedKey, isKeyReleased;
        }

        private sealed class KeyboardCaptureSink : IKeyboardCaptureSink
        {
            private readonly IList<Keystroke> _keystrokes;

            public IList<Keystroke> Keystrokes { get { return _keystrokes; } }

            public KeyboardCaptureSink()
            {
                _keystrokes = new List<Keystroke>();
            }

            void IKeyboardCaptureSink.ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
                _keystrokes.Add(new Keystroke() { keyCode=keyCode, isScanCode=isScanCode, isExtendedKey=isExtendedKey, isKeyReleased=isKeyReleased });
            }
        }

        private KeyboardCaptureSink _sink;
        private CoreWindowKeyboardCore _core;

        [TestInitialize]
        public void SetUpTest()
        {
            _sink = new KeyboardCaptureSink();
            _core = new CoreWindowKeyboardCore(_sink);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _sink = null;
            _core = null;
        }

        [TestMethod]
        public void ModifierDown_ScanCodeDownReported()
        {
            VirtualKey[] keys = new VirtualKey[]
            {
                VirtualKey.LeftShift, VirtualKey.RightShift,
                VirtualKey.LeftControl, VirtualKey.RightControl,
                VirtualKey.LeftMenu, VirtualKey.RightMenu,
            };

            foreach (VirtualKey vk in keys)
            {
                CorePhysicalKeyStatus keyStatus = new CorePhysicalKeyStatus() { ScanCode = (uint)vk, RepeatCount = 1, WasKeyDown = false };
                _core.ProcessKeyDown(vk, keyStatus);
                Assert.AreEqual(1, _sink.Keystrokes.Count);
                Assert.AreEqual((int)vk, _sink.Keystrokes[0].keyCode);
                Assert.IsTrue(_sink.Keystrokes[0].isScanCode);
                Assert.IsFalse(_sink.Keystrokes[0].isKeyReleased);
                _sink.Keystrokes.Clear();
            }
        }

        [TestMethod]
        public void LetterDown_NothingReported()
        {
            _core.ProcessKeyDown(VirtualKey.A, new CorePhysicalKeyStatus() { ScanCode = 10, RepeatCount = 1, WasKeyDown = false });
            Assert.AreEqual(0, _sink.Keystrokes.Count);
        }

        [TestMethod]
        public void LetterDown_Character_CharacterDownReported()
        {
            CorePhysicalKeyStatus keyStatus = new CorePhysicalKeyStatus() { ScanCode = 10, RepeatCount = 1, WasKeyDown = false };
            _core.ProcessKeyDown(VirtualKey.A, keyStatus);
            _core.ProcessCharacterReceived((uint)'a', keyStatus);
            Assert.AreEqual(1, _sink.Keystrokes.Count);
            Assert.AreEqual((int)'a', _sink.Keystrokes[0].keyCode);
            Assert.IsFalse(_sink.Keystrokes[0].isScanCode);
            Assert.IsFalse(_sink.Keystrokes[0].isKeyReleased);
        }

        [TestMethod]
        public void KeyDown_Character_KeyUp_CharacterDownUpReported()
        {
            CorePhysicalKeyStatus keyStatus = new CorePhysicalKeyStatus() { ScanCode = 10, RepeatCount = 1, WasKeyDown = false };
            _core.ProcessKeyDown(VirtualKey.A, keyStatus);
            _core.ProcessCharacterReceived((uint)'a', keyStatus);
            keyStatus.WasKeyDown = true;
            _core.ProcessKeyUp(VirtualKey.A, keyStatus);
            Assert.AreEqual(2, _sink.Keystrokes.Count);
            Assert.AreEqual((int)'a', _sink.Keystrokes[0].keyCode);
            Assert.IsFalse(_sink.Keystrokes[0].isScanCode);
            Assert.IsFalse(_sink.Keystrokes[0].isKeyReleased);
            Assert.AreEqual((int)'a', _sink.Keystrokes[1].keyCode);
            Assert.IsFalse(_sink.Keystrokes[1].isScanCode);
            Assert.IsTrue(_sink.Keystrokes[1].isKeyReleased);
        }

        [TestMethod]
        public void ModifierUp_NothingReported()
        {
            CorePhysicalKeyStatus keyStatus = new CorePhysicalKeyStatus() { ScanCode = 10, RepeatCount = 1, WasKeyDown = false };
            _core.ProcessKeyUp(VirtualKey.LeftShift, keyStatus);
            Assert.AreEqual(0, _sink.Keystrokes.Count);
        }
    }
}
