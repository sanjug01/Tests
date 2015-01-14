namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public interface IKeyboardState
    {
        void PushVirtualKey(VirtualKey virtualKey, IVirtualKey vk);
        IVirtualKey ReleaseVirtualKey(VirtualKey virtualKey);
        void RegisterCharacterKey(CorePhysicalKeyStatus keyStatus, IVirtualKey vk);
        IVirtualKey UnregisterCharacterKey(CorePhysicalKeyStatus keyStatus);
    }
}
