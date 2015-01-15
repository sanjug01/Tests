namespace RdClient.Shared.Input.Keyboard
{
    using Windows.System;
    using Windows.UI.Core;

    /// <summary>
    /// Interface of an event handler of keyboard events for a particular virtual key.
    /// </summary>
    public interface IVirtualKey
    {
        bool Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus);
        void Clear();
    }
}
