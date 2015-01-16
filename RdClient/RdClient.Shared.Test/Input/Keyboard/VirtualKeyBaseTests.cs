namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Collections.Generic;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class VirtualKeyBaseTests
    {
        private sealed class KeyEventArgs : EventArgs
        {
            public readonly CorePhysicalKeyStatus KeyStatus;
            public readonly PhysicalKeyDataContainer DataContainer;
            public bool Handled;

            public KeyEventArgs(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
            {
                KeyStatus = keyStatus;
                DataContainer = dataContainer;
                Handled = false;
            }
        }

        private sealed class CharacterEventArgs : EventArgs
        {
            public readonly char Character;
            public readonly CoreAcceleratorKeyEventType EventType;
            public readonly CorePhysicalKeyStatus KeyStatus;
            public readonly PhysicalKeyDataContainer DataContainer;

            public CharacterEventArgs(char character, CoreAcceleratorKeyEventType eventType, CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
            {
                Character = character;
                EventType = eventType;
                KeyStatus = keyStatus;
                DataContainer = dataContainer;
            }
        }

        private sealed class ProtocolEventArgs : EventArgs
        {
            public readonly int KeyCode;
            public readonly bool IsScanCode, IsExtendedKey, IsKeyReleased;

            public ProtocolEventArgs(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
                KeyCode = keyCode;
                IsScanCode = isScanCode;
                IsExtendedKey = isExtendedKey;
                IsKeyReleased = isKeyReleased;
            }
        }

        private sealed class KeyData : IPhysicalKeyData
        {
        }

        private sealed class TestKey : VirtualKeyBase
        {
            public TestKey(VirtualKey virtualKey, IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
                : base(virtualKey, keyboardSink, keyboardState)
            {
            }

            public event EventHandler<KeyEventArgs> Pressed;
            public event EventHandler<KeyEventArgs> Released;
            public event EventHandler<CharacterEventArgs> Character;

            public VirtualKey TestVirtualKey { get { return this.VirtualKey; } }

            public void TestRegisterPhysical(CorePhysicalKeyStatus keyStatus)
            {
                this.RegisterPhysicalKey(keyStatus);
            }

            public void TestUnregisterPhysical(CorePhysicalKeyStatus keyStatus)
            {
                this.UnregisterPhysicalKey(keyStatus);
            }

            public void TestClearPhysical()
            {
                this.ClearPhysicalKeys();
            }

            public IList<KeyValuePair<CorePhysicalKeyStatus, PhysicalKeyDataContainer>> TestGetPhysicalKeys()
            {
                IList<KeyValuePair<CorePhysicalKeyStatus, PhysicalKeyDataContainer>> list = new List<KeyValuePair<CorePhysicalKeyStatus, PhysicalKeyDataContainer>>();
                this.ForEachPhysicalKey(kvp => list.Add(kvp));
                return list;
            }

            public void TestReportKeyEvent(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
                this.ReportKeyEvent(keyCode, isScanCode, isExtendedKey, isKeyReleased);
            }

            protected override bool OnPressed(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
            {
                KeyEventArgs args = new KeyEventArgs(keyStatus, dataContainer) { Handled = base.OnPressed(keyStatus, dataContainer) };
                if (null != this.Pressed)
                    this.Pressed(this, args);
                return args.Handled;
            }

            protected override void OnReleased(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
            {
                base.OnReleased(keyStatus, dataContainer);
                if (null != this.Released)
                    this.Released(this, new KeyEventArgs(keyStatus, dataContainer));
            }

            protected override void OnCharacter(char character, CoreAcceleratorKeyEventType eventType, CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
            {
                base.OnCharacter(character, eventType, keyStatus, dataContainer);
                if (null != this.Character)
                    this.Character(this, new CharacterEventArgs(character, eventType, keyStatus, dataContainer));
            }
        }

        private sealed class Sink : IKeyboardCaptureSink
        {
            public event EventHandler<ProtocolEventArgs> Protocol;

            void IKeyboardCaptureSink.ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
                if (null != this.Protocol)
                    this.Protocol(this, new ProtocolEventArgs(keyCode, isScanCode, isExtendedKey, isKeyReleased));
            }
        }

        private sealed class State : IKeyboardState
        {
            private readonly IDictionary<CorePhysicalKeyStatus, IVirtualKey> _physicalKeys;
            private readonly IDictionary<VirtualKey, IVirtualKey> _virtualKeys;

            public State()
            {
                _physicalKeys = new SortedDictionary<CorePhysicalKeyStatus, IVirtualKey>(new ScanCodeComparer());
                _virtualKeys = new SortedDictionary<VirtualKey, IVirtualKey>();
            }

            public IList<KeyValuePair<CorePhysicalKeyStatus, IVirtualKey>> GetPhysicalKeys()
            {
                IList<KeyValuePair<CorePhysicalKeyStatus, IVirtualKey>> list = new List<KeyValuePair<CorePhysicalKeyStatus, IVirtualKey>>();
                foreach(var kvp in _physicalKeys)
                    list.Add(kvp);
                return list;
            }

            public IList<KeyValuePair<VirtualKey, IVirtualKey>> GetVirtualKeys()
            {
                IList<KeyValuePair<VirtualKey, IVirtualKey>> list = new List<KeyValuePair<VirtualKey, IVirtualKey>>();
                foreach (var kvp in _virtualKeys)
                    list.Add(kvp);
                return list;
            }

            public void RegisterVirtualKey(VirtualKey virtualKey, IVirtualKey vk)
            {
                _virtualKeys.Add(virtualKey, vk);
            }

            IVirtualKey IKeyboardState.UnregisterVirtualKey(VirtualKey virtualKey)
            {
                IVirtualKey vk;
                if (!_virtualKeys.TryGetValue(virtualKey, out vk))
                    vk = DummyVirtualKey.Dummy;
                else
                    _virtualKeys.Remove(virtualKey);
                return vk;
            }

            void IKeyboardState.RegisterPhysicalKey(CorePhysicalKeyStatus keyStatus, IVirtualKey vk)
            {
                _physicalKeys.Add(keyStatus, vk);
            }

            IVirtualKey IKeyboardState.UnregisterPhysicalKey(CorePhysicalKeyStatus keyStatus)
            {
                Assert.IsTrue(_physicalKeys.ContainsKey(keyStatus));
                IVirtualKey vk = _physicalKeys[keyStatus];
                _physicalKeys.Remove(keyStatus);
                return vk;
            }
        }

        [TestMethod]
        public void NewVirtualKey_ConstructedCorrectly()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);

            Assert.AreEqual(VirtualKey.Enter, key.TestVirtualKey);
            var registered = state.GetVirtualKeys();
            Assert.AreEqual(1, registered.Count);
            Assert.AreEqual(key.TestVirtualKey, registered[0].Key);
            Assert.AreSame(key, registered[0].Value);
        }

        [TestMethod]
        public void VirtualKey_RegisterPhysicalKey_Registered()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 1, IsExtendedKey = false };

            key.TestRegisterPhysical(status);
            var physicalKeys = state.GetPhysicalKeys();
            Assert.AreEqual(1, physicalKeys.Count);
            Assert.AreEqual(status.ScanCode, physicalKeys[0].Key.ScanCode);
            Assert.AreEqual(status.IsExtendedKey, physicalKeys[0].Key.IsExtendedKey);
            Assert.AreSame(key, physicalKeys[0].Value);
        }

        [TestMethod]
        public void VirtualKey_UnregisterPhysicalKey_Unregistered()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 1, IsExtendedKey = false };

            key.TestRegisterPhysical(status);
            key.TestUnregisterPhysical(status);
            var physicalKeys = state.GetPhysicalKeys();
            Assert.AreEqual(0, physicalKeys.Count);
        }

        [TestMethod]
        public void VirtualKey_KeyDown_RegistersPhysicalKeyContainer()
        {
            CoreAcceleratorKeyEventType[] events = new CoreAcceleratorKeyEventType[]
            {
                CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.SystemKeyDown
            };

            foreach (var ev in events)
            {
                State state = new State();
                TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
                IVirtualKey vk = key;
                CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };

                vk.Update(ev, VirtualKey.Enter, status);
                var registered = key.TestGetPhysicalKeys();
                Assert.AreEqual(1, registered.Count);
                Assert.IsNull(registered[0].Value.KeyData);
                Assert.AreEqual(status.ScanCode, registered[0].Key.ScanCode);
                Assert.AreEqual(status.IsExtendedKey, registered[0].Key.IsExtendedKey);
            }
        }

        [TestMethod]
        public void VirtualKey_KeyDown_DefaultHandledFlagReturned()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
            IVirtualKey vk = key;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };

            Assert.IsFalse(vk.Update(CoreAcceleratorKeyEventType.KeyDown, VirtualKey.Enter, status));
        }

        [TestMethod]
        public void VirtualKey_KeyDownRaiseHandled_HandledFlagReturned()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
            IVirtualKey vk = key;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };

            key.Pressed += (sender, e) => e.Handled = true;

            Assert.IsTrue(vk.Update(CoreAcceleratorKeyEventType.KeyDown, VirtualKey.Enter, status));
        }

        [TestMethod]
        public void VirtualKey_KeyDown_OnPressedCalled()
        {
            CoreAcceleratorKeyEventType[] events = new CoreAcceleratorKeyEventType[]
            {
                CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.SystemKeyDown
            };

            foreach (var ev in events)
            {
                State state = new State();
                TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
                IVirtualKey vk = key;
                CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };
                IList<KeyEventArgs> pressedArgs = new List<KeyEventArgs>();
                int releasedCount = 0;

                key.Pressed += (sender, e) => pressedArgs.Add(e);
                key.Released += (sender, e) => ++releasedCount;

                vk.Update(ev, VirtualKey.Enter, status);
                Assert.AreEqual(1, pressedArgs.Count);
                Assert.IsNotNull(pressedArgs[0].DataContainer);
                Assert.IsNull(pressedArgs[0].DataContainer.KeyData);
                Assert.AreEqual(status.ScanCode, pressedArgs[0].KeyStatus.ScanCode);
                Assert.AreEqual(status.IsExtendedKey, pressedArgs[0].KeyStatus.IsExtendedKey);
                Assert.AreEqual(0, releasedCount);
            }
        }

        [TestMethod]
        public void VirtualKey_KeyDownKeyUp_UnregistersPhysicalKeyContainer()
        {
            Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>[] events = new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>[]
            {
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.KeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.SystemKeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.SystemKeyDown, CoreAcceleratorKeyEventType.KeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.SystemKeyDown, CoreAcceleratorKeyEventType.SystemKeyUp),
            };

            foreach (var ev in events)
            {
                State state = new State();
                TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
                IVirtualKey vk = key;
                CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };

                vk.Update(ev.Item1, VirtualKey.Enter, status);
                vk.Update(ev.Item2, VirtualKey.Enter, status);
                var registered = key.TestGetPhysicalKeys();
                Assert.AreEqual(0, registered.Count);
                Assert.AreEqual(0, state.GetVirtualKeys().Count);
            }
        }

        [TestMethod]
        public void VirtualKey_KeyDownKeyUp_OnReleasedCalled()
        {
            Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>[] events = new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>[]
            {
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.KeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.SystemKeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.SystemKeyDown, CoreAcceleratorKeyEventType.KeyUp),
                new Tuple<CoreAcceleratorKeyEventType, CoreAcceleratorKeyEventType>(CoreAcceleratorKeyEventType.SystemKeyDown, CoreAcceleratorKeyEventType.SystemKeyUp),
            };

            foreach (var ev in events)
            {
                State state = new State();
                TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
                IVirtualKey vk = key;
                CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };
                IList<KeyEventArgs> releasedArgs = new List<KeyEventArgs>();
                KeyData keyData = new KeyData();

                key.Pressed += (sender, e) => e.DataContainer.KeyData = keyData;
                key.Released += (sender, e) => releasedArgs.Add(e);

                vk.Update(ev.Item1, VirtualKey.Enter, status);
                status.IsKeyReleased = true;
                vk.Update(ev.Item2, VirtualKey.Enter, status);
                Assert.AreEqual(1, releasedArgs.Count);
                Assert.IsNotNull(releasedArgs[0].DataContainer);
                Assert.AreSame(keyData, releasedArgs[0].DataContainer.KeyData);
                Assert.AreEqual(status.ScanCode, releasedArgs[0].KeyStatus.ScanCode);
                Assert.AreEqual(status.IsExtendedKey, releasedArgs[0].KeyStatus.IsExtendedKey);
                Assert.IsTrue(releasedArgs[0].KeyStatus.IsKeyReleased);
            }
        }

        [TestMethod]
        public void VirtualKey_KeyDownClearPhysicalKeys_PhysicalKeysCleared()
        {
            CoreAcceleratorKeyEventType[] events = new CoreAcceleratorKeyEventType[]
            {
                CoreAcceleratorKeyEventType.KeyDown, CoreAcceleratorKeyEventType.SystemKeyDown
            };

            foreach (var ev in events)
            {
                State state = new State();
                TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
                IVirtualKey vk = key;
                CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 50, IsExtendedKey = false };

                vk.Update(ev, VirtualKey.Enter, status);
                Assert.AreEqual(1, key.TestGetPhysicalKeys().Count);
                key.TestClearPhysical();
                Assert.AreEqual(0, key.TestGetPhysicalKeys().Count);
            }
        }

        [TestMethod]
        public void VirtualKey_2KeysDown_2PhysicalKeysRegistered()
        {
            State state = new State();
            TestKey key = new TestKey(VirtualKey.Enter, new Sink(), state);
            IVirtualKey vk = key;
            CorePhysicalKeyStatus leftShift = new CorePhysicalKeyStatus() { ScanCode = 42, IsExtendedKey = false };
            CorePhysicalKeyStatus rightShift = new CorePhysicalKeyStatus() { ScanCode = 54, IsExtendedKey = false };
            IList<KeyEventArgs> pressedArgs = new List<KeyEventArgs>();
            int releasedCount = 0;

            key.Pressed += (sender, e) => pressedArgs.Add(e);
            key.Released += (sender, e) => ++releasedCount;

            vk.Update(CoreAcceleratorKeyEventType.KeyDown, VirtualKey.Shift, leftShift);
            vk.Update(CoreAcceleratorKeyEventType.KeyDown, VirtualKey.Shift, rightShift);
            Assert.AreEqual(2, pressedArgs.Count);
            Assert.AreEqual(0, releasedCount);

            var physicalKeys = key.TestGetPhysicalKeys();
            Assert.AreEqual(2, physicalKeys.Count);
        }

        [TestMethod]
        public void VirtualKey_2KeysDownClear_AllKeysUnregistered()
        {
            const VirtualKey Key = VirtualKey.Shift;

            State state = new State();
            TestKey key = new TestKey(Key, new Sink(), state);
            IVirtualKey vk = key;
            CorePhysicalKeyStatus leftShift = new CorePhysicalKeyStatus() { ScanCode = 42, IsExtendedKey = false };
            CorePhysicalKeyStatus rightShift = new CorePhysicalKeyStatus() { ScanCode = 54, IsExtendedKey = false };
            int releasedCount = 0;

            key.Released += (sender, e) => ++releasedCount;

            vk.Update(CoreAcceleratorKeyEventType.KeyDown, Key, leftShift);
            vk.Update(CoreAcceleratorKeyEventType.KeyDown, Key, rightShift);
            vk.Clear();

            Assert.AreEqual(2, releasedCount);
            Assert.AreEqual(0, key.TestGetPhysicalKeys().Count);
            Assert.AreEqual(0, state.GetPhysicalKeys().Count);
            Assert.AreEqual(0, state.GetVirtualKeys().Count);
        }

        [TestMethod]
        public void VirtualKey_KeyDownCharacterKeyUp_CharacterReported()
        {
            VirtualKey Key = (VirtualKey)231;
            State state = new State();
            TestKey key = new TestKey(Key, new Sink(), state);
            IVirtualKey vk = key;
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus() { ScanCode = 0, IsExtendedKey = false };
            KeyData keyData = new KeyData();
            bool characterCalled = false;

            key.Pressed += (sender, e) =>
            {
                Assert.IsInstanceOfType(sender, typeof(TestKey));
                ((TestKey)sender).TestRegisterPhysical(e.KeyStatus);
                e.DataContainer.KeyData = keyData;
            };

            key.Character += (sender, e) =>
            {
                characterCalled = true;
                Assert.AreEqual('a', e.Character);
                Assert.AreEqual(CoreAcceleratorKeyEventType.Character, e.EventType);
                Assert.AreSame(keyData, e.DataContainer.KeyData);
                Assert.AreEqual(status.ScanCode, e.KeyStatus.ScanCode);
            };

            vk.Update(CoreAcceleratorKeyEventType.KeyDown, Key, status);
            vk.Update(CoreAcceleratorKeyEventType.Character, (VirtualKey)'a', status);
            Assert.IsTrue(characterCalled);
            status.IsKeyReleased = true;
            vk.Update(CoreAcceleratorKeyEventType.KeyUp, Key, status);
            var registered = key.TestGetPhysicalKeys();
            Assert.AreEqual(0, registered.Count);
            Assert.AreEqual(0, state.GetVirtualKeys().Count);
        }

        [TestMethod]
        public void VirtualKey_ReportKeyEvent_Reported()
        {
            ProtocolEventArgs[] args = new ProtocolEventArgs[]
            {
                new ProtocolEventArgs(100, false, false, false),
                new ProtocolEventArgs(200, false, false, true),
                new ProtocolEventArgs(300, false, true, false),
                new ProtocolEventArgs(400, true, false, false)
            };

            IList<ProtocolEventArgs> reported = new List<ProtocolEventArgs>();
            State state = new State();
            Sink sink = new Sink();
            TestKey key = new TestKey(VirtualKey.Enter, sink, state);

            sink.Protocol += (sender, e) => reported.Add(e);

            foreach(ProtocolEventArgs a in args)
                key.TestReportKeyEvent(a.KeyCode, a.IsScanCode, a.IsExtendedKey, a.IsKeyReleased);

            Assert.AreEqual(args.Length, reported.Count);
            for (int i = 0; i < args.Length; ++i)
            {
                Assert.AreEqual(args[i].KeyCode, reported[i].KeyCode);
                Assert.AreEqual(args[i].IsScanCode, reported[i].IsScanCode);
                Assert.AreEqual(args[i].IsExtendedKey, reported[i].IsExtendedKey);
                Assert.AreEqual(args[i].IsKeyReleased, reported[i].IsKeyReleased);
            }
        }
    }
}
