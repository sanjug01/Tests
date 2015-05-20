namespace RdClient.Shared.ValidationRules
{
    public class UsernameFormatValidationRule : IValidationRule<string>
    {
        public IValidationResult Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new ValidationResult(ValidationResultStatus.NullOrEmpty);
            }
            else if (string.Equals("invalid", value))
            {
                return new ValidationResult(ValidationResultStatus.Invalid, UsernameValidationFailure.InvalidFormat);
            }
            else
            {
                return new ValidationResult(ValidationResultStatus.Valid);
            }            
        }
    }
}
