using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public class MyTerminalExpression : IAbstractExpression
    {
        string _varName;
        MyTerminalExpression(string var)
        {
            _varName = var;
        }

        public int Interpret(IContext context)
        {
            return context.GetValueOf(_varName);
        }
    }

    public class MyNonTerminalExpression : IAbstractExpression
    {
        /// <summary>
        /// implements logic of parsing non-terminal expression         
        /// 
        /// </summary>
        /// <param name="context"> context</param>
        /// <returns>value of the expression</returns>
        public int Interpret(IContext context)
        {
            throw new NotImplementedException();
        }
    }

}
