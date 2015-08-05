using System.Collections.Generic;

namespace RdClient.Shared.Helpers
{
    public class ArrayComparer : IComparer<byte[]>
    {
        public int Compare(byte[] x, byte[] y)
        {
            if (x.Length < y.Length)
                return -1;
            else if (x.Length > y.Length)
                return 1;
            else
            {
                int i;
                for(i = 0; i < x.Length; i++)
                {
                    if (x[i] < y[i])
                        return -1;
                    else if (x[i] > y[i])
                        return 1;
                }
            }

            return 0;
        }
    }
}
