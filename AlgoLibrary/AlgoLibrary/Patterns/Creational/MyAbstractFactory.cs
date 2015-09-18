using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public interface MyAbstractFactory
    {
        AbstractProduct1 CreateProduct1();
        AbstractProduct2 CreateProduct2();
    }
}
