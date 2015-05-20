namespace RdClient.Shared.ValidationRules
{
    public class CharacterOccurenceValidationRule : IValidationRule<string>
    {
        private readonly string _illegalCharacters;
        private readonly object _errorDetail;

        protected string IllegalCharacters { get { return _illegalCharacters; } }

        public CharacterOccurenceValidationRule(string illegalCharacters, object errorDetail = null)
        {
            _illegalCharacters = illegalCharacters;
            _errorDetail = errorDetail;
        }
        public IValidationResult Validate(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.Empty();
            }
            else if (HasIllegalCharacters(stringValue))
            {
                return ValidationResult.Invalid(_errorDetail);
            }
            else
            {
                return ValidationResult.Valid();
            }
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
