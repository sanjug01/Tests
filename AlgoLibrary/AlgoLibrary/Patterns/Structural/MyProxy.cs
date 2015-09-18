using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyProxy : ISubject
    {
        private readonly ISubject _realSubject;
        private int _callCnt;

        public MyProxy(ISubject subject)
        {
            _callCnt = 0;
            _realSubject = subject;
        }

        public int Request(string param)
        {

            System.Diagnostics.Debug.WriteLine("Proxy request with param: {0}", param);
            int result = _realSubject.Request(param);

            // may do some house keeping
            _callCnt++;
            return (_callCnt * 1000 + result); 
        }
    }
}
