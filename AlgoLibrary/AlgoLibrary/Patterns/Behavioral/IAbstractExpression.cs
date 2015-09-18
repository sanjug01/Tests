using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public interface IAbstractExpression
    {
        int Interpret(IContext context);
    }
}
