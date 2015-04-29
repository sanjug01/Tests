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
        private readonly List<IValidationRule<T>> _rules;
        private IEnumerable<object> _errors;
        private bool _isValid;

        public ValidatedProperty(IEnumerable<IValidationRule<T>> rules)
        {
            _rules = new List<IValidationRule<T>>();
            foreach (var rule in rules)
            {
                _rules.Add(rule);
            }
        }

        public ValidatedProperty(IValidationRule<T> rule)
            : this(new List<IValidationRule<T>>() { rule })
        {
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

        public IEnumerable<object> Errors
        {
            get
            {
                return _errors;
            }
            private set
            {
                if (SetProperty(ref _errors, value))
                {
                    SetIsValid();
                }
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
            private set { SetProperty(ref _isValid, value); }
        }

        private void Validate()
        {
            this.Errors = _rules.Select(r => r.Validate(this.Value)).Where(r => !r.IsValid).Select(r => r.ErrorContent);
        }

        private void SetIsValid()
        {
            this.IsValid = !this.Errors?.Any() ?? true;
        }
    }
}
