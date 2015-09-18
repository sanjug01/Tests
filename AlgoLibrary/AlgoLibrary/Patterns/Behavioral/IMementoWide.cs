using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    /// <summary>
    /// Wide memento interface can be used by originator to save the state
    /// </summary>
    public interface IMementoWide : IMementoNarrow
    {
        void AddToState(string state, string value);
    }
}
