using System;

namespace RdClient.Shared.Input.Pointer
{
    public class ConsumptionModeTracker : IConsumptionMode
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
    }
}
