using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Input.Pointer
{
    class PointerVisibilityConsumer : IPointerEventConsumer
    {
        private IRemoteSessionControl _sessionControl;
        public event EventHandler<IPointerEventBase> ConsumedEvent;
        ConsumptionModeType _consumptionMode;
        public ConsumptionModeType ConsumptionMode
        {
            set
            {
                _consumptionMode = value;
            }
        }

        public PointerVisibilityConsumer(ITimerFactory timerFactory, IRemoteSessionControl sessionControl, IPointerPosition pointerPosition, IDeferredExecution dispatcher)
        {
            _sessionControl = sessionControl;
        }
        public void Consume(IPointerEventBase pointerEvent)
        {
            PointerRoutedEventArgsWrapper pointerEventArgs = (PointerRoutedEventArgsWrapper)pointerEvent;
            //When in touch mode pointer should not show up
            if ((_consumptionMode == ConsumptionModeType.DirectTouch || _consumptionMode == ConsumptionModeType.MultiTouch)&&
                (pointerEventArgs.DeviceType != PointerDeviceType.Mouse))
            {
                _sessionControl.RenderingPanel.ChangeMouseVisibility(false);
            }
            else
            {
                
                if (pointerEventArgs.Action == PointerEventAction.PointerEntered)
                {
                    _sessionControl.RenderingPanel.ChangeMouseVisibility(true);
                }
                else if (pointerEventArgs.Action == PointerEventAction.PointerExited && pointerEventArgs.DeviceType == PointerDeviceType.Mouse)
                {
                    _sessionControl.RenderingPanel.ChangeMouseVisibility(false);
                }
            }
        }

        public void Reset()
        {
        }
    }
}
