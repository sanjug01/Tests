using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    /// <summary>
    /// narrow interface to be used by Caretaker
    /// </summary>
    public interface IMementoNarrow
    {
        IContext State { get; set; }
    }
}
