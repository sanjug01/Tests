using System;

namespace RdClient.Shared.ValidationRules
{
    public class ValidationResult : IValidationResult
    {
        private readonly ValidationResultStatus _status;
        private readonly object _content;

        public  ValidationResult(ValidationResultStatus status, object content = null)
        {
            _status = status;
            _content = content;
        }

        public object ErrorContent { get { return _content; } }

        public ValidationResultStatus Status { get { return _status; } }
    }
}
