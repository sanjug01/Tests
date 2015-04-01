namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Input.Keyboard;
    using Windows.System;
    using Windows.UI.Core;

    [TestClass]
    public sealed class VirtualKeyFactoryTests
    {
        private sealed class Sink : IKeyboardCaptureSink
        {
            void IKeyboardCaptureSink.ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
            }
        }

        private sealed class State : IKeyboardState
        {
            void IKeyboardState.RegisterVirtualKey(VirtualKey virtualKey, IVirtualKey vk)
            {
            }

            IVirtualKey IKeyboardState.UnregisterVirtualKey(VirtualKey virtualKey)
            {
                return DummyVirtualKey.Dummy;
            }

            void IKeyboardState.RegisterPhysicalKey(CorePhysicalKeyStatus keyStatus, IVirtualKey vk)
            {
            }

            IVirtualKey IKeyboardState.UnregisterPhysicalKey(CorePhysicalKeyStatus keyStatus)
            {
                return DummyVirtualKey.Dummy;
            }
        }

        private static VirtualKey[] _ignoredKeys = new VirtualKey[]
        {
            VirtualKey.None, VirtualKey.LeftWindows, VirtualKey.RightWindows
        };

        [TestMethod]
        public void VirtualKeyFactory_IgnoredKeys_DummyVirtualKey()
        {
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 0,
                RepeatCount = 1
            };

            foreach(VirtualKey vk in _ignoredKeys)
            {
                IVirtualKey key = factory.MakeVirtualKey(vk, status, new State());
                Assert.IsInstanceOfType(key, typeof(DummyVirtualKey));
            }
        }

        [TestMethod]
        public void VirtualKeyFactory_Emoticon_CharacterKey()
        {
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 0,
                RepeatCount = 1
            };

            IVirtualKey key = factory.MakeVirtualKey((VirtualKey)231, status, new State());
            Assert.IsInstanceOfType(key, typeof(CharacterKey));
        }

        [TestMethod]
        public void VirtualKeyFactory_LeftShift_SingleReleaseKey()
        {
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 42,
                RepeatCount = 1
            };

            IVirtualKey key = factory.MakeVirtualKey(VirtualKey.Shift, status, new State());
            Assert.IsInstanceOfType(key, typeof(SingleReleaseKey));
        }

        [TestMethod]
        public void VirtualKeyFactory_RightShift_SingleReleaseKey()
        {
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 54,
                RepeatCount = 1
            };

            IVirtualKey key = factory.MakeVirtualKey(VirtualKey.Shift, status, new State());
            Assert.IsInstanceOfType(key, typeof(SingleReleaseKey));
        }

        [TestMethod]
        public void VirtualKeyFactory_G_ExtendedKey()
        {
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 34,
                RepeatCount = 1
            };

            IVirtualKey key = factory.MakeVirtualKey(VirtualKey.G, status, new State());
            Assert.IsInstanceOfType(key, typeof(ExtendedKey));
        }
    }
}
