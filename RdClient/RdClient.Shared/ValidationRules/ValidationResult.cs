namespace RdClient.Shared.ValidationRules
{
    public class ValidationResult : IValidationResult
    {
        private readonly ValidationResultStatus _status;
        private readonly object _content;

        private  ValidationResult(ValidationResultStatus status, object content = null)
        {
            _status = status;
            _content = content;
        }

        public static ValidationResult Valid()
        {
            return new ValidationResult(ValidationResultStatus.Valid);
        }

        public static ValidationResult Invalid(object content = null)
        {
            return new ValidationResult(ValidationResultStatus.Invalid, content);
        }

        public static ValidationResult Empty()
        {
            return new ValidationResult(ValidationResultStatus.Empty);
        }

        public object ErrorContent { get { return _content; } }

        public ValidationResultStatus Status { get { return _status; } }
    }
}
