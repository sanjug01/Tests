using System;

namespace RdClient.Shared.Input.Pointer
{
    public class ConsumptionModeTracker : IConsumptionModeTracker, IInputDeviceTracker
    {
        private ConsumptionModeType _consumptionMode;
        public ConsumptionModeType ConsumptionMode
        {
            get
            {
                return _consumptionMode;
            }

            set
            {
                _consumptionMode = value;
                EmitConsumptionModeChanged(_consumptionMode);
            }
        }

        private void EmitConsumptionModeChanged(ConsumptionModeType newMode)
        {
            if(ConsumptionModeChanged != null)
            {
                ConsumptionModeChanged(this, newMode);
            }
        }

        public event EventHandler<ConsumptionModeType> ConsumptionModeChanged;

        private InputDeviceType _inputDeviceType;
        public InputDeviceType InputDevice
        {
            get
            {
                return _inputDeviceType;
            }

            set
            {
                _inputDeviceType = value;
                EmitInputDeviceChanged(_inputDeviceType);
            }
        }

        private void EmitInputDeviceChanged(InputDeviceType newDevice)
        {
            if (ConsumptionModeChanged != null)
            {
                InputDeviceChanged(this, newDevice);
            }
        }

        public event EventHandler<InputDeviceType> InputDeviceChanged;
    }
}
