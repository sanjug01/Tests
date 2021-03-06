﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class ArraysAlgorithms
    {
        public ArraysAlgorithms() { }
        public int BaseMethod()
        {
            return 1;
        }

        public int[] ProductExceptSelf(int[] nums)
        {
            int N = nums.Length;
            int[] result = new int[N];

            // pre-product
            result[0] = 1;
            for(int i=1;i<N;i++)
            {
                result[i] = result[i-1] * nums[i];
            }

            // post-product
            for(int i = N-2; i>=0;i--)
            {
                nums[i] = nums[i] * nums[i + 1];
                result[i] = result[i] * nums[i];
            }

            return result;
        }

        public int[] PlusOne(int[] digits)
        {
            bool passOne = true;
            int passLast = digits.Length - 1;

            while (passLast >= 0 && passOne)
            {
                if (digits[passLast] + 1 < 10)
                    passOne = false;
                else
                    passLast--;
            }

            int[] result;
            if (passOne)
            {
                result = new int[digits.Length + 1];
                result[0] = 1;

                for (int i = 0; i < digits.Length; i++)
                    result[i + 1] = 0;
            }
            else
            {
                result = new int[digits.Length];
                for (int i = passLast + 1; i < digits.Length; i++)
                    result[i] = 0;
                for (int i = 0; i < passLast; i++)
                    result[i] = digits[i];

                result[passLast] = digits[passLast] + 1;
            }

            return result;
        }

        public bool isPalindrome(int n)
        {
            if (n < 0) return false;

            // count how many digits
            int digits = 0;
            int m = n;
            while (m > 0)
            {
                m = m / 10;
                digits++;
            }

            m = 0;
            for(int i = 0; i< digits/2; i++)
            {
                m = 10 * m + n % 10;
                n = n / 10;
            }

            if(1 == digits %2)
            {
                n = n / 10;
            }

            return m == n ;
        }

        public string ZigZagConvert(string s, int numRows)
        {
            if (null == s) return null;

            char[] source = s.ToCharArray();
            if (1 == numRows) return new string(source);

            int N = source.Length;
            char[] target = new char[N];

            int rowOffset = 2 * numRows - 2;
            int maxOffset = rowOffset;
            int targetIdx = 0;

            for (int i = 0; i < numRows; i++)
            {
                rowOffset = 2 * i;
                
                for (int j = i; j < N && targetIdx < N; j = j + rowOffset)
                {
                    target[targetIdx++] = source[j];
                    if (rowOffset < maxOffset) rowOffset = maxOffset - rowOffset; // alternates for all but first and last
                }

            }

            return new string(target);
        }

        public int MissingNumber(int[] nums)
        {
            int result = 0;
            int N = nums.Length;
            for(int i=0;i<N; i++)
            {
                result = result ^ i;
                result = result ^ nums[i];
            }

            result = result ^ N;

            return result;
        }

        public int FirstMissingPositive(int[] nums)
        {
            int tmp;
            int N = nums.Length;

            int idx = 1;
            while(idx <= N)
            {
                if (nums[idx - 1] == idx)
                {
                    // in place, advance
                    idx++;
                }
                else if (nums[idx-1] > 0 && nums[idx-1] != idx && nums[idx-1] <= N)
                {
                    // swap, but no advance, unless a duplicate
                    tmp = nums[idx - 1];
                    if (nums[tmp - 1] == tmp)
                    {
                        nums[idx - 1] = 0;
                        idx++;
                    }
                    else
                    {
                        nums[idx - 1] = nums[tmp - 1];
                        nums[tmp - 1] = tmp;
                    }
                }
                else
                {
                    // invalid, set to 0 all invalid values
                    nums[idx-1] = 0;
                    idx++;
                }
            }

            // search first with 0
            for(int i=0; i<N; i++)
            {
                if (nums[i] == 0)
                    return i + 1;
            }

            return N+1;

        }

        public void MatrixRotation(long[,] A, int n)
        {
            // rotates a square matrix in place
            for (int i = 0; i < n / 2; i++)
            {
                // rotate line i - > column n-i-1 -> line (n-i-1) reversed- column (i) reversed
                // make sure the last element is not rotated twice
                for (int j = i; j < n -i -1 ; j++)
                {
                    long temp = A[n - j - 1, i];
                    A[n - j - 1, i] = A[n - i - 1, n - j - 1];
                    A[n - i - 1, n - j - 1] = A[j, n-i -1];
                    A[j, n - i - 1] = A[i, j];
                    A[i, j] = temp;
                }
            }
        } 


        private int FindRangeInSubarray(long[] A, int value, int start, int end, out int min, out int max)
        {
            min = -1;
            max = -1;

            if (start > end) return 0;
            if (start == end)
            {
                if (A[start] == value)
                {
                    min = max = start;
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            
            // binary search
            int mid = (start + end) / 2;
            if(A[mid] < value)
            {
                // right side
                return FindRangeInSubarray(A, value, mid+1, end, out min, out max);
            }
            else if (A[mid] > value)
            {
                // left side
                return FindRangeInSubarray(A, value, start, mid -1, out min, out max);
            }
            else
            {
                // found at least one, both left and right
                int leftMin, leftMax, rightMin, rightMax;
                FindRangeInSubarray(A, value, start, mid - 1, out leftMin, out leftMax);
                FindRangeInSubarray(A, value, mid+1, end, out rightMin, out rightMax);

                min = (leftMin >= 0) ? leftMin : mid;
                max = (rightMax >= 0) ? rightMax : mid;
                return 1;
            }

            // return 1; // one for found, 0 otherwise
        }

        public int FindRange(long[] A, int value, out int min, out int max)
        {
            min = -1;
            max = -1;

            return FindRangeInSubarray(A, value, 0, A.Length, out min, out max);
        }

        public static IEnumerator<int> RunLength(int[] _values)
        {
            int value = 0;
            int cnt = 0;

            for(int i=0;i<_values.Length /2; i++)
            {
                value = _values[2 * i + 1];
                cnt = _values[2 * i];
                for (int j=0;j<cnt;j++)
                    yield return value;
            }
        }




        // run length Encoding
        /*
        [1,1,1,2] -> [3,1,1,2]

        */

        public class MyRunLengthEnumerator : IEnumerator<int>
        {
            int[] _values;
            int _crt;
            int _value;
            int _crtSubIndex;

            public MyRunLengthEnumerator(int[] values )
            {
                // if odd number will just ignore last 
                _values = values;
                _crt = 0;
                _crtSubIndex = -1;
                _value = 0; ;
            }

            public int Current
            {
                get
                { 
                    return _value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _value;
                }
            }

            public void Dispose()
            {
            }

            bool HasMore()
            {
                return (_crt <= _values.Length - 2);
            }

            public bool MoveNext()
            {
                while (HasMore() && _crtSubIndex < 0)
                {
                    if (_crtSubIndex < _values[_crt] - 1)
                    {
                        _crtSubIndex++;
                    }
                    else
                    {
                        _crt += 2;
                        _crtSubIndex = -1;
                    }
                }

                if(!HasMore())
                {
                    return false;
                }

                _value = _values[_crt + 1];
                return true;
            }



            public void Reset()
            {
                _crt = 0;
                _crtSubIndex = -1;
                _value = 0;
            }
        }

    }

    public class MyCollection : IEnumerable<int>
    {
        int[] _values;
        public MyCollection(int[] values)
        {
            _values = values;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ArraysAlgorithms.RunLength(_values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ArraysAlgorithms.RunLength(_values);
        }
    }
}
