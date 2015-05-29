using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Input.Pointer
{
    class PointerVisibilityConsumer : IPointerEventConsumer
    {
        private readonly IRemoteSessionControl _sessionControl;
        private ConsumptionModeType _consumptionMode;


        public event EventHandler<IPointerEventBase> ConsumedEvent;
        public void SetConsumptionMode(ConsumptionModeType consumptionMode)
        {
            _consumptionMode = consumptionMode;
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
                _sessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
            }
            else
            {
                
                if (pointerEventArgs.Action == PointerEventAction.PointerEntered)
                {
                    _sessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Visible);
                }
                else if (pointerEventArgs.Action == PointerEventAction.PointerExited && pointerEventArgs.DeviceType == PointerDeviceType.Mouse)
                {
                    _sessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
                }
            }
        }

        public void Reset()
        {
        }
    }
}
