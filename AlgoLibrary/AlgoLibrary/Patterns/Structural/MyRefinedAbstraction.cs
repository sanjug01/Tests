using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyRefinedAbstraction : BridgeAbstractionBase
    {
        // implementor is part of the base
        // it is public only for testing reasons
        public IBridgeImplementor Implementor
        {
            get
            {
                return _implementor;
            } 
            set
            {
                _implementor = value;
            }
        }

        public MyRefinedAbstraction(IBridgeImplementor impl) : base(impl)
        {
            System.Diagnostics.Debug.WriteLine("Using MyRefinedAbstraction");
        }
    }
}
