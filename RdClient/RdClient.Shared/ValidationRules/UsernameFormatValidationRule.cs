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
            else
            {
                return ValidationResult.Valid();
            }            
        }
    }
}
