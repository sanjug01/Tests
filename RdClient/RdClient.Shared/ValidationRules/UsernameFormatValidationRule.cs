namespace RdClient.Shared.ValidationRules
{
    public class UsernameFormatValidationRule : IValidationRule<string>
    {
        public IValidationResult Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Empty();
            }
            else if (string.Equals("invalid", value))
            {
                return ValidationResult.Invalid(UsernameValidationFailure.InvalidFormat);
            }
            else
            {
                return ValidationResult.Valid();
            }            
        }
    }
}
