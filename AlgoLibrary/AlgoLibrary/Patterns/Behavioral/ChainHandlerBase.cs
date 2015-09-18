using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    /// <summary>
    /// chain of command
    /// </summary>
    public abstract class ChainHandlerBase
    {
        public ChainHandlerBase()
        {
            Next = null;
        }

        public ChainHandlerBase Next { get; set; }

        public abstract int HandleRequest(int Request);
    }
}
