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
    public class PointerVisibilityConsumer : IPointerEventConsumer
    {
        private readonly IRenderingPanel _renderingPanel;
        private ConsumptionModeType _consumptionMode;


        public event EventHandler<IPointerEventBase> ConsumedEvent;
        public void SetConsumptionMode(ConsumptionModeType consumptionMode)
        {
            _consumptionMode = consumptionMode;
        }

        public PointerVisibilityConsumer( IRenderingPanel renderingPanel)
        {
            _renderingPanel = renderingPanel;
        }
        public void Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties pointerEventArgs = (IPointerRoutedEventProperties)pointerEvent;
                //When in touch mode pointer should not show up
                if ((_consumptionMode == ConsumptionModeType.DirectTouch || _consumptionMode == ConsumptionModeType.MultiTouch) &&
                    (pointerEventArgs.DeviceType != PointerDeviceType.Mouse))
                {
                    _renderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
                }
                else
                {

                    if (pointerEventArgs.Action == PointerEventAction.PointerEntered)
                    {
                        _renderingPanel.ChangeMouseVisibility(Visibility.Visible);
                    }
                    else if (pointerEventArgs.Action == PointerEventAction.PointerExited && pointerEventArgs.DeviceType == PointerDeviceType.Mouse)
                    {
                        _renderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
                    }
                }
            }
        }

        public void Reset()
        {
        }
    }
}
