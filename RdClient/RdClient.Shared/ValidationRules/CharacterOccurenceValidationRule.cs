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
            ValidationResultStatus status = ValidationResultStatus.Invalid;

            if (string.IsNullOrEmpty(stringValue))
            {
                status = ValidationResultStatus.NullOrEmpty;
            }
            else if (!HasIllegalCharacters(stringValue))
            {
                status = ValidationResultStatus.Valid;
            }

            return new ValidationResult(status);
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
