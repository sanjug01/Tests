using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    /// <summary>
    /// uses memento's wide interface to save the state
    /// </summary>
    public class MyMementoOriginator
    {
        // state as dictionary (optional)
        private Dictionary<string, string> _state;

        // restore state from given memento
        void SetMemento(MyMemento m)
        {
            _state.Clear();

            // TODO: use m.State to restore _state;
        }

        IMementoWide CreateMemento()
        {
            MyMemento m = new MyMemento();

            foreach (var pair in _state)
            {
                m.AddToState(pair.Key, pair.Value);
            }

            return m;
        }
    }
}
