using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared
{
    public struct ScreenSize
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public interface IPhysicalScreenSize
    {
        ScreenSize GetScreenSize();
    }
}
