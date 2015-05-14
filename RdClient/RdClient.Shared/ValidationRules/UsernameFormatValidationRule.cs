namespace RdClient.Shared.ValidationRules
{
    public class UsernameFormatValidationRule : IValidationRule<string>
    {
        public IValidationResult Validate(string value)
        {
            return new ValidationResult(!string.IsNullOrEmpty(value), UsernameValidationFailure.InvalidFormat);
        }
    }
}
