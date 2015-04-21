using System;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerRoutedEventProperties : IPointerEventBase
    {
        PointerDeviceType DeviceType { get; }
        UInt32 PointerId { get; }
        ulong Timestamp { get; }
        bool LeftButton { get; }
        bool RightButton { get; }
        int MouseWheelDelta { get; }
        bool IsHorizontalWheel { get; }
    }
}
