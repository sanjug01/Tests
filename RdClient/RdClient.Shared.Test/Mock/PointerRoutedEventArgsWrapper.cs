using RdClient.Shared.Input.Pointer;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class PointerRoutedEventArgsWrapper : MockBase, IPointerRoutedEventProperties
    {
        public PointerEventAction Action { get; set; }

        public PointerDeviceType DeviceType { get; set; }

        public bool IsHorizontalWheel { get; set; }

        public bool LeftButton { get; set; }

        public int MouseWheelDelta { get; set; }

        public uint PointerId { get; set; }

        public Point Position { get; set; }

        public bool RightButton { get; set; }

        public ulong Timestamp { get; set; }
    }
}
