using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ValidationRules
{
    public class UsernameValidationRule : IValidationRule<string>
    {
        private CharacterOccurenceValidationRule _illegalCharacterValidationRule;

        public UsernameValidationRule()
        {
            _illegalCharacterValidationRule = new CharacterOccurenceValidationRule("/[]\":;|<>+=,?*%");
        }

        public IValidationResult Validate(string stringValue)
        {
            bool result = string.IsNullOrEmpty(stringValue) == false &&
                            _illegalCharacterValidationRule.Validate(stringValue).IsValid && 
                            stringValue.Count(x => x == '\\') < 2;
            return new ValidationResult(result);
        }
    }
}
