using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public class MyPrefixDecorator : IComponent
    {
        private readonly IComponent _component;
        private readonly string _prefix;

        public MyPrefixDecorator(string prefix, IComponent component)
        {
            _prefix = prefix;
            _component = component;
        }

        public string Operation(string param)
        {
            System.Diagnostics.Debug.WriteLine("Adding prefix" + _prefix);
            return _prefix + _component.Operation(param);
        }
    }

    public class MyPostfixDecorator : IComponent
    {
        private readonly IComponent _component;
        private readonly string _postfix;

        public MyPostfixDecorator(string postfix, IComponent component)
        {
            _postfix = postfix;
            _component = component;
        }

        public string Operation(string param)
        {
            System.Diagnostics.Debug.WriteLine("Adding postfix" + _postfix);
            return _component.Operation(param) + _postfix ;
        }
    }

}
