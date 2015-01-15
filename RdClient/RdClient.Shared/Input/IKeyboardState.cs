namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public interface IKeyboardState
    {
        void RegisterVirtualKey(VirtualKey virtualKey, IVirtualKey vk);

        IVirtualKey UnregisterVirtualKey(VirtualKey virtualKey);

        void RegisterPhysicalKey(CorePhysicalKeyStatus keyStatus, IVirtualKey vk);

        IVirtualKey UnregisterPhysicalKey(CorePhysicalKeyStatus keyStatus);
    }
}
