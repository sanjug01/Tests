using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MySubject : ISubject
    {
        public int Request(string param)
        {
            System.Diagnostics.Debug.WriteLine("Request to MySubject with param: {0}", param);
            return 1;
        }
    }
}
