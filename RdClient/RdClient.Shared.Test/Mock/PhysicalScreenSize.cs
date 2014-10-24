using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class PhysicalScreenSize : IPhysicalScreenSize
    {
        private Tuple<int, int> _size;
        public PhysicalScreenSize(Tuple<int, int> size)
        {
            _size = size;
        }

        public Tuple<int, int> GetScreenSize()
        {
            return _size;
        }
    }
}
