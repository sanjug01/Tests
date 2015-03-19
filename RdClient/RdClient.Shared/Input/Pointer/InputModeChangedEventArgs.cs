namespace RdClient.Shared.Input.Pointer
{
    using System;

    public sealed class InputModeChangedEventArgs : EventArgs
    {

        ConsumptionMode _mode;

        public ConsumptionMode Mode { get { return _mode; } }

        public InputModeChangedEventArgs(ConsumptionMode mode)
        {
            _mode = mode;
        }
    }
}
