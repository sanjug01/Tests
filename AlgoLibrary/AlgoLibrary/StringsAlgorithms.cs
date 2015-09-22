using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class StringsAlgorithms
    {
        public StringsAlgorithms() { }

        public class Solution
        {
            private bool IsPartialMatch(string s, string p, int idxS, int idxP)
            {
                if (idxP == p.Length)
                    return (idxS >= s.Length - 1);

                // lookahead for *
                bool isStart = (idxP <= p.Length - 2) && ('*' == p[idxP]);

                if (!isStart)
                {
                    if ('.' != p[idxP])
                    {
                        if (s[idxS] != p[idxP]) return false;
                        return IsPartialMatch(s, p, idxS + 1, idxP + 1);
                    }
                    else
                    {
                        return IsPartialMatch(s, p, idxS + 1, idxP + 1);
                    }
                }
                else
                {
                    if ('.' != p[idxP])
                    {
                        for (int nextIdx = idxS; nextIdx < s.Length && s[nextIdx] == p[idxP]; nextIdx++)
                        {
                            if (IsPartialMatch(s, p, nextIdx, idxP + 2))
                                return true;
                        }
                        return false;
                    }
                    else
                    {
                        for (int nextIdx = idxS; nextIdx < s.Length; nextIdx++)
                        {
                            if (IsPartialMatch(s, p, nextIdx, idxP + 2))
                                return true;
                        }
                        return false;
                    }
                }

            }

            // TODO: not working - needs testing
            public bool IsMatch(string s, string p)
            {
                return IsPartialMatch(s, p, 0, 0);
            }
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
