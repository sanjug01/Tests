using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public interface IBridgeAbstraction
    {

        int Operation(object o);
    }

    /// <summary>
    /// base class has a reference to an Implementor
    /// </summary>
    public class BridgeAbstractionBase : IBridgeAbstraction
    {
        protected IBridgeImplementor _implementor;

        public BridgeAbstractionBase(IBridgeImplementor implementor)
        {
            _implementor = implementor;
        }

        public int Operation(object o)
        {
            return _implementor.OperationImpl(o.ToString());
        }
    }
}
