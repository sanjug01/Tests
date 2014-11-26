using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ValidationRules
{
    public class UsernameValidationRule : IllegalCharacterValidationRule
    {
        public UsernameValidationRule() : base("/\\[]\":;|<>+=,?*%")
        {
        }
    }
}
