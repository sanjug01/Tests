namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public sealed class DummyVirtualKey : IVirtualKey
    {
        bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            return true;
        }


        void IVirtualKey.Clear()
        {
        }
    }
}
