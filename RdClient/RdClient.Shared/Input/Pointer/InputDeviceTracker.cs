using System;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class InputDeviceTracker : IInputDeviceTracker
    {
        private PointerDeviceType _inputDeviceType;
        public PointerDeviceType InputDevice
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

        private void EmitInputDeviceChanged(PointerDeviceType newDevice)
        {
            if (InputDeviceChanged != null)
            {
                InputDeviceChanged(this, newDevice);
            }
        }

        public event EventHandler<PointerDeviceType> InputDeviceChanged;
    }
}
