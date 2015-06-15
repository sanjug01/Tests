namespace RdClient.Shared.Input.Pointer
{
    using RdClient.Shared.Models;
    using System;
    using Windows.Devices.Input;
    using Windows.UI.Xaml;

    public class PointerVisibilityConsumer : IPointerEventConsumer
    {
        private readonly IRenderingPanel _renderingPanel;
        private ConsumptionModeType _consumptionMode;
        private EventHandler<IPointerEventBase> _consumedEvent;


        public event EventHandler<IPointerEventBase> ConsumedEvent
        {
            add { _consumedEvent += value; }
            remove { _consumedEvent -= value; }
        }

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
