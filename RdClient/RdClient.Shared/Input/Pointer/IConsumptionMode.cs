using System;

namespace RdClient.Shared.Input.Pointer
{
    public enum ConsumptionModeType
    {
        Pointer,
        DirectTouch,
        MultiTouch
    }

    public interface IConsumptionMode
    {
        ConsumptionModeType ConsumptionMode { get; set; }

        event EventHandler<ConsumptionModeType> ConsumptionModeChanged;
    }
}
