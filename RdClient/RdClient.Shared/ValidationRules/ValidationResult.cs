using System;

namespace RdClient.Shared.ValidationRules
{
    class ValidationResult : IValidationResult
    {
        private readonly bool _valid;
        private readonly object _content;

        public  ValidationResult(bool valid, object content = null)
        {
            _valid = valid;
            _content = content;
        }

        public bool IsValid { get { return _valid; } }

        public object ErrorContent { get { return _content; } }        
    }
}
