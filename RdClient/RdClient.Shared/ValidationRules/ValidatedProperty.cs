namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Helpers;

    public class ValidatedProperty<T> : MutableObject, IValidatedProperty<T>
    {
        private T _value;
        private readonly IValidationRule<T> _rule;
        private IValidationResult _state;

        public ValidatedProperty(IValidationRule<T> rule)
        {
            _rule = rule;
            _state = new ValidationResult(ValidationResultStatus.NullOrEmpty);
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetProperty(ref _value, value);                
                SetState();                
            }
        }

        public IValidationResult State
        {
            get
            {
                return _state;
            }
            private set
            {
                SetProperty(ref _state, value);
            }
        }

        private void SetState()
        {
            this.State = _rule.Validate(this.Value);
        }
    }
}
