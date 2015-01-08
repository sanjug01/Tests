using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ValidationRules
{
    public class UsernameValidationRule : IValidationRule
    {
        private CharacterOccurenceValidationRule _illegalCharacterValidationRule;

        public UsernameValidationRule()
        {
            _illegalCharacterValidationRule = new CharacterOccurenceValidationRule("/[]\":;|<>+=,?*%");
        }

        public bool Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string stringValue = value as string;

            return 
                string.IsNullOrEmpty(stringValue) == false &&
                _illegalCharacterValidationRule.Validate(stringValue, cultureInfo) && 
                stringValue.Count(x => x == '\\') < 2;
        }
    }
}
