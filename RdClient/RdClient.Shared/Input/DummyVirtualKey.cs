namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public sealed class DummyVirtualKey : IVirtualKey
    {
        private static IVirtualKey _dummy;

        public static IVirtualKey Dummy
        {
            get
            {
                if (null == _dummy)
                    _dummy = new DummyVirtualKey();
                return _dummy;
            }
        }

        bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            return true;
        }


        void IVirtualKey.Clear()
        {
        }
    }
}
