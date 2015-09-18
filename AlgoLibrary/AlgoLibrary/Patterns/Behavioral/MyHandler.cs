using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public class MyHandlerA : ChainHandlerBase
    {
        private int _index;
        public MyHandlerA(int index)
        {
            _index = index;
        }

        public override int HandleRequest(int Request)
        {
            if(_index == Request)
            {
                return 1000 * _index;
            }
            else if(null != this.Next)
            {
                return this.Next.HandleRequest(Request);
            }
            else
            {
                // no handler found
                return -1;
            }
        }
    }

    public class MyHandlerB : ChainHandlerBase
    {
        private int _index;
        public MyHandlerB(int index)
        {
            _index = index;
        }

        public override int HandleRequest(int Request)
        {
            if (_index == Request)
            {
                return 1000000 * _index;
            }
            else if (null != this.Next)
            {
                return this.Next.HandleRequest(Request);
            }
            else
            {
                // no handler found
                return -1;
            }
        }
    }
}
