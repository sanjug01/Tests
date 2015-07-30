using System;

namespace RdClient.Shared.Input.Pointer
{
    public enum ConsumptionModeType
    {
        Pointer,
        DirectTouch,
        MultiTouch
    }

    public interface IConsumptionModeTracker
    {
        ConsumptionModeType ConsumptionMode { get; set; }
        event EventHandler<ConsumptionModeType> ConsumptionModeChanged;
    }

    public interface IInputDeviceTracker
    {
        InputDeviceType InputDevice { get; set; }
        event EventHandler<InputDeviceType> InputDeviceChanged;
    }
}
