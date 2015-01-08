using System.Globalization;

namespace RdClient.Shared.ValidationRules
{
    public class CharacterOccurenceValidationRule : IValidationRule
    {
        private readonly string _illegalCharacters;
        protected string IllegalCharacters { get { return _illegalCharacters; } }

        public CharacterOccurenceValidationRule(string illegalCharacters)
        {
            _illegalCharacters = illegalCharacters;
        }
        public bool Validate(object value, CultureInfo cultureInfo)
        {
            string stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                if (HasIllegalCharacters(stringValue))
                {
                    return false;
                }
            }
            return true;
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
