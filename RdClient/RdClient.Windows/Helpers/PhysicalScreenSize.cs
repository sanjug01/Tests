using RdClient.Shared;
using System;

namespace RdClient.Helpers
{
    class PhysicalScreenSize : IPhysicalScreenSize
    {

        public Tuple<int, int> GetScreenSize()
        {
            return new Tuple<int, int>(52, 87);
        }
    }
}