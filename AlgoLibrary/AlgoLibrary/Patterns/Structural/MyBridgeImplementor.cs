using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyBridgeImplementorA : IBridgeImplementor
    {
        public int OperationImpl(string param)
        {
            System.Diagnostics.Debug.WriteLine("MyBridgeImplementorA operation with param: {0}", param);
            return 1;
        }
    }

    public class MyBridgeImplementorB : IBridgeImplementor
    {
        public int OperationImpl(string param)
        {
            System.Diagnostics.Debug.WriteLine("MyBridgeImplementorB operation with param: {0}", param);
            return 2;
        }
    }
}
