using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public abstract class MyPrototype
    {
        public abstract MyPrototype Clone();
    }

    public class MyConcretePrototypeA : MyPrototype
    {
        public MyConcretePrototypeA() : this("testData")
        { }

        protected MyConcretePrototypeA(string data)
        {
            Data = data;
        }

        public string Data { get; set; }

        public override MyPrototype Clone()
        {
            return new MyConcretePrototypeA(this.Data);
        }
    }

    public class MyConcretePrototypeB : MyPrototype
    {
        public MyConcretePrototypeB() : this(1,100)
        {

        }

        protected MyConcretePrototypeB(int i, int j)
        {
            Min = i;
            Max = j;
        }

        int Min { get; set; }
        int Max { get; set; }
        public override MyPrototype Clone()
        {
            return new MyConcretePrototypeB(Min, Max);
        }
    }
}
