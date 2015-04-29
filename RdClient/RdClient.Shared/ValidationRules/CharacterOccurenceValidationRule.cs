using System;
using System.Globalization;

namespace RdClient.Shared.ValidationRules
{
    public class CharacterOccurenceValidationRule : IValidationRule<string>
    {
        private readonly string _illegalCharacters;
        protected string IllegalCharacters { get { return _illegalCharacters; } }

        public CharacterOccurenceValidationRule(string illegalCharacters)
        {
            _illegalCharacters = illegalCharacters;
        }
        public IValidationResult Validate(string stringValue)
        {
            bool result = true;

            if (!string.IsNullOrEmpty(stringValue))
            {
                if (HasIllegalCharacters(stringValue))
                {
                    result = false;
                }
            }

            return new ValidationResult(result);
        }

        private bool HasIllegalCharacters(string hostName)
        {
            for (int i = 0; i < IllegalCharacters.Length; i++)
            {
                if (hostName.IndexOf(IllegalCharacters[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
