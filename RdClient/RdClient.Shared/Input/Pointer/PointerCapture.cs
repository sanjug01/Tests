﻿using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Input
{
    public class PointerCapture : IPointerCapture, IPointerManipulator
    {
        public IExecutionDeferrer ExecutionDeferrer { private get; set; }        
        public IRemoteSessionControl RemoteSessionControl { private get; set; }
        public IRenderingPanel RenderingPanel { private get; set; }

        public void OnPointerChanged(object sender, PointerEventArgs args)
        {
            // consume the pointer event
        }

        public void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {
            ExecutionDeferrer.DeferToUI(() => {
                ImageSource image = MouseCursorShape.ByteArrayToBitmap(args.Buffer, args.Width, args.Height);
                MouseCursorShape cursor = new MouseCursorShape(new Point(args.XHotspot, args.YHotspot), image);
                this.RenderingPanel.MouseCursorShape = cursor;
            });
        }

        public double MouseAcceleration { get { return (double)1.4; } }

        public Point MousePosition { get; set; }

        public void SendMouseAction(MouseEventType eventType)
        {
            // this.RemoteSessionControl.SendMouseAction
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            // this.RemoteSessionControl.SendMouseWheel
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            // this.RemoteSessionControl.SendTouchAction
        }
    }
}
