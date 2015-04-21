using Windows.Devices.Input;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerRoutedEventArgsCopy : IPointerRoutedEventProperties
    {
        public PointerEventAction Action { get; private set; }
        public PointerDeviceType DeviceType { get; private set; }
        public uint PointerId { get; private set; }
        public Point Position { get; private set; }
        public ulong Timestamp { get; private set; }
        public bool LeftButton { get; private set; }
        public bool RightButton { get; private set; }
        public int MouseWheelDelta { get; private set; }
        public bool IsHorizontalWheel { get; private set; }

        public PointerRoutedEventArgsCopy(IPointerRoutedEventProperties wrapper)
        {
            Action = wrapper.Action;
            DeviceType = wrapper.DeviceType;
            PointerId = wrapper.PointerId;
            Position = wrapper.Position;
            Timestamp = wrapper.Timestamp;
            LeftButton = wrapper.LeftButton;
            RightButton = wrapper.RightButton;
            MouseWheelDelta = wrapper.MouseWheelDelta;
            IsHorizontalWheel = wrapper.IsHorizontalWheel;
        }
    }
}
