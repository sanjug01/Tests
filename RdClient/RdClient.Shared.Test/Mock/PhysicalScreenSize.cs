using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class PhysicalScreenSize : MockBase, IPhysicalScreenSize
    {
        public PhysicalScreenSize(Tuple<int, int> size)
        {
        }

        public Tuple<int, int> GetScreenSize()
        {
            return (Tuple<int, int>)Invoke(new object[] { });
        }
    }
}
