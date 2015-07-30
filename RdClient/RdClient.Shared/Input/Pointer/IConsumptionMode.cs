using System;
using Windows.Devices.Input;

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
        PointerDeviceType InputDevice { get; set; }
        event EventHandler<PointerDeviceType> InputDeviceChanged;
    }
}
