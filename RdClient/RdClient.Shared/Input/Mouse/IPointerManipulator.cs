﻿using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerManipulator
    {
        double MouseAcceleration { get; set; }
        Point MousePosition { get; set; }
        void SendMouseAction(MouseEventType eventType);
        void SendMouseWheel(int delta, bool isHorizontal);
        void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime);

        // TODO: not sure we need this
        //void SendPinchZoomAction();
        //void SendPanAction();
    }
}
