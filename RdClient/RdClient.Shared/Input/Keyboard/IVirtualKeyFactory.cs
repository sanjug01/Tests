namespace RdClient.Shared.Input.Keyboard
{
    using Windows.System;
    using Windows.UI.Core;

    public interface IVirtualKeyFactory
    {
        IVirtualKey MakeVirtualKey(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus, IKeyboardState keyboardState);
    }
}
