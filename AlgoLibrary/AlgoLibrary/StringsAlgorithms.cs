using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class StringsAlgorithms
    {
        public StringsAlgorithms() { }
        public int BaseMethod()
        {
            return 1;
        }

        public int MyAtoi(string str)
        {

            if (null == str || str.Length == 0)
                return 0;

            str = str.Trim();
            int start = 0;
            long result = 0;
            bool isNegative = false;
            if (str[start] == '-')
            {
                start++;
                isNegative = true;
            }
            else if (str[start] == '+')
            {
                start++;
            }

            for (int i = start; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                    return (int)(isNegative ? (-result) : result);

                result = result * 10 + (int)(str[i] - '0');
                if (result > int.MaxValue)
                {
                    return isNegative ? (int.MinValue) : int.MaxValue; ;
                }
            }


            return (int)(isNegative ? (-result) : result);
        }

    }
}
