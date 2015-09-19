using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AlgoLibrary
{
    public class HashmapsAlgorithms
    {
        public HashmapsAlgorithms() { }


        public int[] SingleNumber(int[] nums)
        {
            HashSet<int> once = new HashSet<int>();

            foreach(var v in nums)
            {
                if(once.Contains(v))
                {
                    once.Remove(v);
                }
                else
                {
                    once.Add(v);
                }                
            }

            return once.ToArray();
            
        }

        private int NumSquaresFromDict(int n, Dictionary<int, int> map)
        {
            if (map.ContainsKey(n))
            {
                return map[n];
            }

            // if(n == (int)Math.Sqrt(n) * (int)Math.Sqrt(n))
            // {
            //    map[n] = 1;
            //    return 1;
            // }

            int minLen = n;
            for (int i = 1; i <= n / 2; i++)
            {
                int left = NumSquaresFromDict(i, map);
                int right = NumSquaresFromDict(n - i, map);

                if (minLen > left + right)
                    minLen = left + right;
            }

            map[n] = minLen;
            return minLen;
        }

        public int NumSquares(int n)
        {

            if (n <= 0) return 0;
            if (n == 1) return 1;

            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 1; i * i <= n; i++)
                dict[i * i] = 1;


            /*** populate first, to avoid duplications **/
            for (int i = 1; i <= n; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    NumSquaresFromDict(i, dict);
                }
            }

            return dict[n];
            /*****/

            /* let recursion add more 
            return NumSquaresFromDict(n, dict);
            */
        }


        public int NumSquaresNotRecursive(int n)
        {
            //TODO: there is a more optimum solution I found later
            return 0;
        }

    public int[] StringDecompositions(string sentence, string[] words)
        {
            Queue<int> results = new Queue<int>();
            Dictionary<string, int> wordDictionary = new Dictionary<string, int>();

            int wordSize = words[0].Length;
            int wordCnt = words.Length;

            foreach (string w in words)
            {
                int cntWord = 0;
                wordDictionary.TryGetValue(w, out cntWord);
                wordDictionary[w] = cntWord + 1;
            }

            for (int i = 0; sentence.Length >= i + wordCnt * wordSize; i++)
            {
                if (MatchAllWordsInTheDictionary(sentence, wordDictionary, i, wordCnt, wordSize))
                {
                    results.Enqueue(i);
                }
            }

            return results.ToArray();
        }

        private bool MatchAllWordsInTheDictionary(string s, Dictionary<String, int> dict, int start, int cntWords, int wordSize)
        {
            Dictionary<String, int> searchedDictionary = new Dictionary<string, int>();

            for (int i = 0; i < cntWords; i++)
            {
                string crtWord = s.Substring(start + i * wordSize, wordSize);
                if (!dict.ContainsKey(crtWord))
                {
                    return false;
                }


                int cntWord = 0;
                searchedDictionary.TryGetValue(crtWord, out cntWord);
                searchedDictionary[crtWord] = cntWord + 1;

                if (searchedDictionary[crtWord] > dict[crtWord])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Test Collatz for all numbers less than n
        /// </summary>
        /// <param name="n"></param>
        /// <returns>true if it holds</returns>
        public bool TestCollatz(int n)
        {
            HashSet<long> verified = new HashSet<long>();

            // Original loop: for(long i=2; i<=n; i++)
            // can be optimized by eliminating all even numbers checks, which reduce imediately to n/2
            for (long i = 3; i <= n; i += 2)
            {
                // all ints up to i have been verified
                HashSet<long> checkSequence = new HashSet<long>();
                long crt = i;

                while (crt >= i)
                {
                    if (0 == crt % 2)
                    {
                        // no need to store even numbers
                        crt = crt / 2;
                    }
                    else
                    {

                        // if (verified.Contains(crt))
                        if (!verified.Add(crt))
                        {
                            break;
                        }

                        if (!checkSequence.Add(crt))
                        {
                            // already in checkSequence means infinite loop
                            return false;
                        }

                        try
                        {
                            crt = 3 * crt + 1; // possible overflow
                        }
                        catch (OverflowException exc)
                        {
                            System.Diagnostics.Debug.WriteLine("Cannot verify because of overflow");
                            throw exc;
                        }
                    }
                }

                // sequence is valid, reached either less than i or verified value
                // verified.UnionWith(checkSequence);
            }

            return true;
        }


        public bool TestCollatzRange(int min, int max, HashSet<long> verified)
        {
            // minimum should be at least 2
            if (min <= 1) min = 2;
            int start = (0 == min % 2) ? (min + 1) : min;

            // Original loop: for(long i=2; i<=n; i++)
            // can be optimized by eliminating all even numbers checks, which reduce imediately to n/2
            for (long i = start; i <= max; i += 2)
            {
                // all ints up to i have been verified
                HashSet<long> checkSequence = new HashSet<long>();
                long crt = i;

                while (crt >= i)
                {
                    if (0 == crt % 2)
                    {
                        // no need to store even numbers
                        crt = crt / 2;
                    }
                    else
                    {
                        // better option verify & add - no need to union
                        // if (verified.Contains(crt))
                        if (!verified.Add(crt))
                        {
                            break;
                        }

                        if (!checkSequence.Add(crt))
                        {
                            // already in checkSequence means infinite loop
                            return false;
                        }

                        try
                        {
                            crt = 3 * crt + 1; // possible overflow
                        }
                        catch (OverflowException exc)
                        {
                            System.Diagnostics.Debug.WriteLine("Cannot verify because of overflow");
                            throw exc;
                        }
                    }
                }

                // sequence is valid, reached either less than i or verified value
                // verified.UnionWith(checkSequence); - no longer needed
            }

            return true;
        }


        public int LongestDistinctSubarray(int[] array)
        {
            Dictionary<int, int> lastPos = new Dictionary<int, int>();
            int crtSubstringStartIdx = 0;
            int crtSubstringLen = 0;
            int longestLen = 0;

            for(int i=0; i < array.Length; i++)
            {
                crtSubstringLen = i - crtSubstringStartIdx;
                if (lastPos.ContainsKey(array[i]))
                {
                    int prevPos = lastPos[array[i]];
                    if(prevPos >= crtSubstringStartIdx)
                    {
                        longestLen = Math.Max(longestLen, crtSubstringLen);
                        crtSubstringStartIdx = prevPos + 1;
                    }
                }
                else
                {
                    // longestLen++;

                }
                lastPos[array[i]] = i;
            }

            // reached the end
            crtSubstringLen = array.Length - crtSubstringStartIdx;
            longestLen = Math.Max(longestLen, crtSubstringLen);

            return longestLen;
        }

        public int LongestContainedRange(int[] array)
        {
            HashSet<int> notProcessed = new HashSet<int>();
            int longestRange = 0;

            foreach(var value in array)
            {
                notProcessed.Add(value);
            }

            while(notProcessed.Count > 0)
            {
                // process one element from the set
                int crt = notProcessed.First();
                notProcessed.Remove(crt);
                int crtLen = 1;

                int left = crt - 1;
                while (notProcessed.Remove(left))
                {
                    crtLen++;
                    left--;
                }

                int right = crt + 1;
                while(notProcessed.Remove(right))
                {
                    crtLen++;
                    right++;
                }

                if (longestRange < crtLen)
                    longestRange = crtLen;
            }

            return longestRange;
        }

        private int nextInt(int n)
        {
            int result = 0;
            while (n > 0)
            {
                result += (n % 10) * (n % 10);
                n = n / 10;
            }

            return result;
        }

        public bool IsHappy(int n)
        {
            // the number will quickly decrease to have 3 digits
            // next for any 3 digit number will be less than 9^2+9^2+9^2 = 243
            HashSet<int> notHappy = new HashSet<int>();

            if (1 == n) return true;

            while (n > 1)
            {
                if (notHappy.Contains(n))
                    return false;

                if (n <= 243) notHappy.Add(n);
                n = nextInt(n);
            }

            return true;
        }
    }

}
