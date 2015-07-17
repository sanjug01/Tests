using System;

namespace RdClient.Shared.Helpers
{
    public class ArrayRound
    {
        private int[] _values;

        public ArrayRound(int[] values)
        {
            _values = values;
        }

        public int Round(int value)
        {
            if (_values.Length < 1)
                return value;

            int choice = _values[0];
            int delta = Math.Abs(_values[0] - value);

            foreach(int valid in _values)
            {
                int newDelta = Math.Abs(valid - value); 
                if(newDelta < delta)
                {
                    delta = newDelta;
                    choice = valid;
                }
            }

            return choice;
        }
    }
}
