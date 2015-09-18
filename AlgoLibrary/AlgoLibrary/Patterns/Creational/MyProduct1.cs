using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public class MyProduct1 : AbstractProduct1
    {
        protected MyProduct1() { }
        public MyProduct1(string name)
        {
            P1Name = name;
        }

        public string P1Name
        {
            get;  set;
        }
    }

    public class MyProduct2 : AbstractProduct2
    {
        public string P2Name
        {
            get; private set;
        }

        protected MyProduct2() { }

        public MyProduct2(string name)
        {
            P2Name = name;
        }
    }


    public class MyProductOne : MyProduct1
    {
        public MyProductOne() : base("ONE")
        {
        }
    }

    public class MyProductTwo : MyProduct2
    {
        public MyProductTwo() : base("TWO")
        {
        }
    }
}
