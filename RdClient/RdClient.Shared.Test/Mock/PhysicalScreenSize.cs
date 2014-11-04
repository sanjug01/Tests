using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class PhysicalScreenSize : MockBase, IPhysicalScreenSize
    {
        public PhysicalScreenSize(ScreenSize size)
        {
        }

        public ScreenSize GetScreenSize()
        {
            return (ScreenSize)Invoke(new object[] { });
        }
    }
}
