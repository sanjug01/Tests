namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public interface IVirtualKeyFactory
    {
        IVirtualKey MakeVirtualKey(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus);
    }
}
