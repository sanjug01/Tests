using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns
{
    public interface MyBuilder
    {
        void BuildPart(string part);
    }

    public class MyConcreteBuilder : MyBuilder
    {
        private StringBuilder _description = new StringBuilder("House");
        private bool _isContainer = true;
        public void BuildPart(string part)
        {
            if (_isContainer)
            {
                _description.Append(" has a ");
            }
            else
            {
                _description.Append(". I see a ");
            }

            _description.Append(part);
            if (String.Compare("wall", part, StringComparison.OrdinalIgnoreCase) == 0)
                _isContainer = true;
            else
                _isContainer = false;
        }

        public string GetResult()
        {
            return _description.ToString();
        }
    }

}
