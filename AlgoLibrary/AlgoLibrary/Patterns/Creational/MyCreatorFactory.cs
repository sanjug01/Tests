using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public abstract class MyCreatorFactory
    {
        // it has an abstract factory method
        public abstract MyProduct1 CreateProduct1();

        public string UseMyProduct()
        {
            MyProduct1 prod = this.CreateProduct1();
            System.Diagnostics.Debug.WriteLine("using factory method to create prod1: {0}", prod.P1Name);
            return prod.P1Name;
        }
    }

    public class MyConcreteCreatorFactoryA : MyCreatorFactory
    {
        public override MyProduct1 CreateProduct1()
        {
            return new MyProduct1("By FactoryMethod A");
        }
    }

    public class MyConcreteCreatorFactoryB : MyCreatorFactory
    {
        public override MyProduct1 CreateProduct1()
        {
            return new MyProductOne();
        }
    }
}
