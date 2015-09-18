using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    /// <summary>
    /// This adapter is holding a reference to an adaptee object
    /// </summary>
    public class MyObjectAdapter : ITarget
    {
        private readonly IAdaptee _adaptee;

        public MyObjectAdapter(IAdaptee ad)
        {
            _adaptee = ad;
        }

        public int Request(string requestString)
        {
            return _adaptee.SpecificRequest(requestString);
        }
    }
}
