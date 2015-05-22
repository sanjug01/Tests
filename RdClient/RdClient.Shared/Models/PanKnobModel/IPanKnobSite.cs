﻿using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.Viewport;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnobSite : IPointerEventConsumer
    {
        ITimerFactory TimerFactory { get; }
        IViewport Viewport { set; }

        IPanKnob PanKnob { get; set; }

        void OnConsumptionModeChanged(object sender, ConsumptionModeType consumptionMode);
    }
}
