namespace RdClient.Shared.Test.Input
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Input;
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
            IVirtualKeyFactory factory = new VirtualKeyFactory(new Sink(), new State());
            CorePhysicalKeyStatus status = new CorePhysicalKeyStatus()
            {
                ScanCode = 0,
                RepeatCount = 1
            };

            foreach(VirtualKey vk in _ignoredKeys)
            {
                IVirtualKey key = factory.MakeVirtualKey(vk, status);
                Assert.IsInstanceOfType(key, typeof(DummyVirtualKey));
            }
        }
    }
}
