using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared
{
    public interface IPhysicalScreenSize
    {
        Tuple<int,int> GetScreenSize();
    }
}
