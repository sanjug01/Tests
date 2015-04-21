using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public interface IGestureRoutedEventProperties : IPointerEventBase
    {
        PointerDeviceType DeviceType { get; }
        uint Count { get; }
    }
}
