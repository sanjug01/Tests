using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ValidationRules
{
    public class ValidatedProperty<T> : MutableObject, IValidatedProperty<T>
    {
        private T _value;
        private readonly IValidationRule<T> _rule;
        private IValidationResult _state;
        private bool _enableValidation;

        public ValidatedProperty(IValidationRule<T> rule)
        {
            _rule = rule;
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    Validate();
                }
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

        public bool EnableValidation
        {
            get
            {
                return _enableValidation;
            }
            set
            {
                if (SetProperty(ref _enableValidation, value))
                {
                    Validate();
                }
            }
        }

        private void Validate()
        {
            if (this.EnableValidation)
            {
                this.State = _rule.Validate(this.Value);
            }
            else
            {
                this.State = new ValidationResult(true);
            }
        }
    }
}
