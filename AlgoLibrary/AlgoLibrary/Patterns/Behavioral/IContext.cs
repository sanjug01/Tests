using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public interface IContext
    {
        int GetValueOf(string var);
    }

    public class MyContext : IContext
    {
        Dictionary<string, int> _dictionary;

        public MyContext()
        {
            _dictionary = new Dictionary<string, int>();
        }

        ~MyContext()
        {
            _dictionary.Clear();
        }
        public int GetValueOf(string var)
        {
            return _dictionary[var];
        }

        public void AddValue(string var, int value)
        {
            _dictionary[var] = value;
        }
    }
}
