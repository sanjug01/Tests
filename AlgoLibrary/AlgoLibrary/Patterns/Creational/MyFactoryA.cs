using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public class MyFactoryA : MyAbstractFactory
    {
        public AbstractProduct1 CreateProduct1()
        {
            var prod = new MyProduct1("By FactoryA");

            System.Diagnostics.Debug.WriteLine("Creating P1 named {0}", prod.P1Name);
            return prod;
        }
        

        AbstractProduct2 MyAbstractFactory.CreateProduct2()
        {
            var prod =  new MyProduct2("By FactoryA");

            System.Diagnostics.Debug.WriteLine("Creating P2 named {0}", prod.P2Name);
            return prod;
        }
    }

    public class MyFactoryB : MyAbstractFactory
    {
        public AbstractProduct1 CreateProduct1()
        {
            var prod = new MyProductOne();

            System.Diagnostics.Debug.WriteLine("Factory B creating P1 named {0}", prod.P1Name);
            return prod;
        }


        AbstractProduct2 MyAbstractFactory.CreateProduct2()
        {
            var prod = new MyProductTwo();

            System.Diagnostics.Debug.WriteLine("FactoryB creating P2 named {0}", prod.P2Name);
            return prod;
        }
    }
    }
