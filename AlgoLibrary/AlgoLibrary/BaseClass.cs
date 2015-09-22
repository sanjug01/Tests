using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class CostObject<T> : IComparable<CostObject<T>>
    {
        public T Instance { get; private set; }
        public double Cost { get; set; }

        public CostObject(T obj, double cost)
        {
            Instance = obj;
            Cost = cost;
        }


        public int CompareTo(CostObject<T> other)
        {
            return this.Cost.CompareTo(other.Cost);
        }
    }


    public class BaseClass
    {
        public static Byte LeastBitOnly(Byte b)
        {
            return (Byte)(b & ~(b - 1));
        }


        public static Byte ResetLeastBit(Byte b)
        {
            return (Byte)(b & (b - 1));
        }

        public static UInt16 LeastBitOnly(UInt16 c)
        {
            return (UInt16) (c & ~(c - 1));
        }

        public static UInt16 ResetLeastBit(UInt16 c)
        {
            return (UInt16)(c & (c - 1));
        }


        public static uint BitCount(UInt16 c)
        {
            uint i = 0;
            UInt16 lb = 0;
            for (lb = LeastBitOnly(c); lb != 0; i++, lb = LeastBitOnly(c))
            {
                c -= lb;
            }

            return i;
        }

        public static int RangeBitwiseAnd(int m, int n)
        {
            // it is the common bit prefix
            int result = 0;
            int bit = 1 << (8 * sizeof(int)-1);
            for (int i = 0; i < 8 * sizeof(int); i++, bit = bit >> 1)
            {
                if ((m & bit) != (n & bit))
                    break;
                result += (m & bit);
            }

            return result;
        }

        // inefficient
        public static uint BitCount2(UInt16 c)
        {
            int i = 0;
            for(int j=0; j<16 && c!=0; j++, c >>= 1)
            {
                i += c & 1;
            }

            return (uint) i;
        }


        Char IntToChar(Int16 iVal)
        {
            return (Char) iVal;
        }

        Int16 CharToInt(Char ch)
        {
            return Convert.ToInt16(ch);
        }
        // TODO:
        // 1. test if a tree is BST
        // 2. generate BST from preorder (unique elements)
    }
}
