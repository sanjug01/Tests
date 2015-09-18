using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyAdaptee : IAdaptee
    {
        public int SpecificRequest(object o)
        {
            System.Diagnostics.Debug.WriteLine("New request for MyAdaptee: {0}", o.ToString());
            return 100;
        }
    }
}
