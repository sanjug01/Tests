namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Collections.Generic;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class KeyboardStateTests
    {
        private sealed class MakeVirtualKeyEventArgs : EventArgs
        {
            public readonly VirtualKey VirtualKey;
            public readonly CorePhysicalKeyStatus KeyStatus;
            public readonly IKeyboardState KeyboardState;
            private IVirtualKey _key;

            public IVirtualKey Key { get { return _key; } }

            public void SetKey(IVirtualKey key)
            {
                Assert.IsNull(_key);
                Assert.IsNotNull(key);
                _key = key;
            }

            public MakeVirtualKeyEventArgs(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus, IKeyboardState keyboardState)
            {
                VirtualKey = virtualKey;
                KeyStatus = keyStatus;
                KeyboardState = keyboardState;
                _key = null;
            }
        }

        private sealed class KeyFactory : IVirtualKeyFactory
        {
            public event EventHandler<MakeVirtualKeyEventArgs> MakeKey;

            IVirtualKey IVirtualKeyFactory.MakeVirtualKey(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus, IKeyboardState keyboardState)
            {
                Assert.IsNotNull(this.MakeKey);
                MakeVirtualKeyEventArgs args = new MakeVirtualKeyEventArgs(virtualKey, keyStatus, keyboardState);

                this.MakeKey(this, args);
                Assert.IsNotNull(args.Key);
                return args.Key;
            }
        }

        private sealed class VirtualKeyEventArgs : EventArgs
        {
            public readonly CoreAcceleratorKeyEventType EventType;
            public readonly VirtualKey VirtualKey;
            public readonly CorePhysicalKeyStatus KeyStatus;
            public bool Handled;

            public VirtualKeyEventArgs(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey,  CorePhysicalKeyStatus keyStatus)
            {
                EventType = eventType;
                VirtualKey = virtualKey;
                KeyStatus = keyStatus;
                Handled = false;
            }
        }

        private class TestKey : IVirtualKey
        {
            public event EventHandler Cleared;
            public event EventHandler<VirtualKeyEventArgs> Updated;

            bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
            {
                VirtualKeyEventArgs e = new VirtualKeyEventArgs(eventType, virtualKey, keyStatus);
                if (null != this.Updated)
                    this.Updated(this, e);
                return e.Handled;
            }

            void IVirtualKey.Clear()
            {
                if (null != this.Cleared)
                    this.Cleared(this, EventArgs.Empty);
            }
        }

        private TestSink _sink;
        private KeyFactory _keyFactory;
        private KeyboardState _state;
        private IKeyboardState _iState;

        [TestInitialize]
        public void SetUpTest()
        {
            _sink = new TestSink();
            _keyFactory = new KeyFactory();
            _state = new KeyboardState(_sink, _keyFactory);
            _iState = _state;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _sink = null;
            _keyFactory = null;
            _state = null;
            _iState = null;
        }

        [TestMethod]
        public void KeyboardState_KeyDown_RequestsNewKey()
        {
            const VirtualKey Key = VirtualKey.Enter;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };
            int keyCount = 0;

            _keyFactory.MakeKey += (sender, e) =>
            {
                ++keyCount;
                Assert.AreEqual(Key, e.VirtualKey);
                Assert.AreEqual(status.ScanCode, e.KeyStatus.ScanCode);
                e.SetKey(DummyVirtualKey.Dummy);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            Assert.AreEqual(1, keyCount);
        }

        [TestMethod]
        public void KeyboardState_RegisterUnregisterVirtualKey_SameKeyUnregisters()
        {
            TestKey key1 = new TestKey(),
                    key2 = new TestKey();

            _iState.RegisterVirtualKey(VirtualKey.Enter, key1);
            _iState.RegisterVirtualKey(VirtualKey.Shift, key2);
            Assert.AreSame(key1, _iState.UnregisterVirtualKey(VirtualKey.Enter));
            Assert.AreSame(key2, _iState.UnregisterVirtualKey(VirtualKey.Shift));
        }

        [TestMethod]
        public void KeyboardState_RegisterUnregisterUnregisterVirtualKey_DifferentSecondUnregistered()
        {
            TestKey key = new TestKey();

            _iState.RegisterVirtualKey(VirtualKey.Enter, key);
            Assert.AreSame(key, _iState.UnregisterVirtualKey(VirtualKey.Enter));
            Assert.AreNotSame(key, _iState.UnregisterVirtualKey(VirtualKey.Enter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void KeyboardState_RegisterRegisterVirtualKey_ThrowsException()
        {
            TestKey key = new TestKey();

            _iState.RegisterVirtualKey(VirtualKey.Enter, key);
            _iState.RegisterVirtualKey(VirtualKey.Enter, key);
        }

        [TestMethod]
        public void KeyboardState_RegisterUnregisterPhysicalKey_UnregistersSameKey()
        {
            TestKey key = new TestKey();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _iState.RegisterPhysicalKey(status, key);
            Assert.AreSame(key, _iState.UnregisterPhysicalKey(status));
        }

        [TestMethod]
        public void KeyboardState_RegisterUnregisterUnregisterPhysicalKey_DifferentSecondKey()
        {
            TestKey key = new TestKey();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _iState.RegisterPhysicalKey(status, key);
            Assert.AreSame(key, _iState.UnregisterPhysicalKey(status));
            Assert.AreNotSame(key, _iState.UnregisterPhysicalKey(status));
        }

        [TestMethod]
        public void KeyboardState_RegisterSamePhysicalKey_Registers()
        {
            TestKey key = new TestKey();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _iState.RegisterPhysicalKey(status, key);
            _iState.RegisterPhysicalKey(status, key);
            Assert.AreSame(key, _iState.UnregisterPhysicalKey(status));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void KeyboardState_RegisterDuplicatePhysicalKey_ThrowsException()
        {
            TestKey
                key1 = new TestKey(),
                key2 = new TestKey();
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };

            _iState.RegisterPhysicalKey(status, key1);
            _iState.RegisterPhysicalKey(status, key2);
        }

        [TestMethod]
        public void KeyboardState_RegisterVirtualKeyClear_KeyClearedAndUnregistered()
        {
            const VirtualKey Key = VirtualKey.Enter;
            TestKey key = null;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };
            int clearCount = 0;

            _keyFactory.MakeKey += (sender, e) =>
            {
                key = new TestKey();

                key.Cleared += (senderCleared, eCleared) =>
                {
                    Assert.AreSame(senderCleared, key);
                    Assert.AreSame(senderCleared, _iState.UnregisterVirtualKey(Key));
                    ++clearCount;
                };

                e.SetKey(key);
                _iState.RegisterVirtualKey(e.VirtualKey, key);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            Assert.IsNotNull(key);
            _state.Clear();
            Assert.AreNotSame(key, _iState.UnregisterVirtualKey(Key));
            Assert.AreEqual(1, clearCount);
        }

        [TestMethod]
        public void KeyboardState_DeadCharacter_FullyProcessed()
        {
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };
            Assert.IsTrue(_state.UpdateState(CoreAcceleratorKeyEventType.DeadCharacter, VirtualKey.Enter, status));
            Assert.IsTrue(_state.UpdateState(CoreAcceleratorKeyEventType.SystemDeadCharacter, VirtualKey.Enter, status));
        }

        [TestMethod]
        public void KeyboardState_KeyDownCharacter_CharacterReported()
        {
            const VirtualKey Key = VirtualKey.A;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50 };
            TestKey key = null;
            IList<VirtualKeyEventArgs> keyEvents = new List<VirtualKeyEventArgs>();

            _keyFactory.MakeKey += (sender, e) =>
            {
                key = new TestKey();
                key.Updated += (senderUpdated, eUpdated) =>
                {
                    keyEvents.Add(eUpdated);

                    switch(eUpdated.EventType)
                    {
                        case CoreAcceleratorKeyEventType.KeyDown:
                            Assert.IsInstanceOfType(senderUpdated, typeof(IVirtualKey));
                            _iState.RegisterPhysicalKey(eUpdated.KeyStatus, (IVirtualKey)senderUpdated);
                            break;
                    }
                };
                e.SetKey(key);
            };

            _state.UpdateState(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            Assert.IsNotNull(key);
            _state.UpdateState(CoreAcceleratorKeyEventType.Character, (VirtualKey)'a', status);
            Assert.AreEqual(2, keyEvents.Count);
            Assert.AreEqual(CoreAcceleratorKeyEventType.Character, keyEvents[1].EventType);
            Assert.AreEqual('a', (char)keyEvents[1].VirtualKey);
            Assert.AreEqual(status.ScanCode, keyEvents[1].KeyStatus.ScanCode);
        }
    }
}
