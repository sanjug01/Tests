using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RdClient.Shared.ValidationRules
{
    public class ValidatedProperty<T> : MutableObject, IValidatedProperty<T>
    {
        private T _value;
        private readonly IValidationRule<T> _rule;
        private IValidationResult _state;
        private bool _showErrors;
        private Visibility _errorVisible;

        public ValidatedProperty(IValidationRule<T> rule)
        {
            _rule = rule;
            _showErrors = true;
            Validate();
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
                if (SetProperty(ref _state, value))
                {
                    SetErrorVisible();
                }
            }
        }

        public bool ShowErrors
        {
            get
            {
                return _showErrors;
            }

            set
            {
                if (SetProperty(ref _showErrors, value))
                {
                    SetErrorVisible();
                }
            }
        }

        private void SetErrorVisible()
        {
            Visibility errorVisibility = Visibility.Collapsed;
            if (this.ShowErrors && !this.State.IsValid)
            {
                errorVisibility = Visibility.Visible;
            }
            this.ErrorVisible = errorVisibility;
        }

        public Visibility ErrorVisible
        {
            get
            {
                return _errorVisible;
            }
            private set
            {
                SetProperty(ref _errorVisible, value);
            }
        }

        private void Validate()
        {
            this.State = _rule.Validate(this.Value);
        }
    }
}
