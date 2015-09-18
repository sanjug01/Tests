using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyComponent : IComponent
    {
        private readonly string _data;

        public MyComponent(string data)
        {
            _data = data;
        }

        public string Operation(string param)
        {
            System.Diagnostics.Debug.WriteLine("MyComponent oper({0})", param);
            return _data;
        }
    }
}
