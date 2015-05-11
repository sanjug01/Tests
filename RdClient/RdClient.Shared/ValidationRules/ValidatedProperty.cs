namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Helpers;

    public class ValidatedProperty<T> : MutableObject, IValidatedProperty<T>
    {
        private T _value;
        private readonly IValidationRule<T> _rule;
        private IValidationResult _state;

        public ValidatedProperty(IValidationRule<T> rule, T initialValue)
        {
            _value = initialValue;
            _rule = rule;
            _state = new ValidationResult(true);
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

        public bool ValidateNow()
        {
            SetState();
            return this.State?.IsValid ?? false;
        }
    }
}
