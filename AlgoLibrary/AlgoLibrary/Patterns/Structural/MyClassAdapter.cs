using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    /// <summary>
    /// MyClassAdapter1 implements 2 separate interfaces
    /// </summary>
    public class MyClassAdapter1 : ITarget, IAdaptee
    {
        string _lastRequest;

        public int Request(string requestString)
        {
            return this.SpecificRequest(requestString);
        }

        // maybe protected but interface does not allow it.
        public int SpecificRequest(object o)
        {
            _lastRequest = o.ToString();
            System.Diagnostics.Debug.WriteLine("MyClassAdapter1's SpecificRequest invoked: {0}", _lastRequest);
            return 1;
        }
    }

    /// <summary>
    /// MyClassAdapter2 implements an interface and extends AdapteeBase
    /// The advantage is it can hide Adaptee interface
    /// </summary>
    public class MyClassAdapter2 : AdapteeBase, ITarget 
    {
        String _lastRequest;
        public int Request(string requestString)
        {
            return this.SpecificRequest(requestString);
        }

        protected override int SpecificRequest(object o)
        {
            _lastRequest = o.ToString();
            System.Diagnostics.Debug.WriteLine("MyClassAdapter2's SpecificRequest invoked: {0}", _lastRequest);
            return 2;
        }
    }
}
