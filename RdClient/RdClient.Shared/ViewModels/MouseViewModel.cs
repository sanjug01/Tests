using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using MousePointer = Tuple<int, float, float>;

    public class MouseViewModel : MutableObject
    {
        private MousePointer _mousePointer;
        public MousePointer MousePointer { 
            get { return _mousePointer; } 
            set {
                SetProperty(ref _mousePointer, value);
                OnMousePointerChanged();
            } 
        }

        private IRdpConnection _rdpConnection;
        public IRdpConnection RdpConnection { set { _rdpConnection = value; } }

        private void OnMousePointerChanged()
        {
            float xPos = this.MousePointer.Item2;
            float yPos = this.MousePointer.Item3;
            MouseEventType eventType = MouseEventType.Unknown;

            switch(this.MousePointer.Item1)
            {
                case(0):
                    eventType = MouseEventType.LeftPress;
                    break;
                case (1):
                    eventType = MouseEventType.LeftRelease;
                    break;
                case (2):
                    eventType = MouseEventType.MouseHWheel;
                    break;
                case (3):
                    eventType = MouseEventType.MouseWheel;
                    break;
                case (4):
                    eventType = MouseEventType.Move;
                    break;
                case (5):
                    eventType = MouseEventType.RightPress;
                    break;
                case (6):
                    eventType = MouseEventType.RightRelease;
                    break;
                default:
                    eventType = MouseEventType.Unknown;
                    break;
            }

            if(_rdpConnection != null)
            {
                _rdpConnection.SendMouseEvent(eventType, xPos, yPos);            
            }
        }
    }
}
