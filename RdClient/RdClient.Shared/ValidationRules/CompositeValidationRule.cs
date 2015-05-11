namespace RdClient.Shared.ValidationRules
{
    using System.Collections.Generic;

    public class CompositeValidationRule<T> : IValidationRule<T>
    {
        private readonly List<IValidationRule<T>> _rules;

        public CompositeValidationRule(IEnumerable<IValidationRule<T>> rules)
        {
            _rules = new List<IValidationRule<T>>(rules);
        }

        //Return validation result for first rule that fails, or if none fail then return a passing validation result
        public IValidationResult Validate(T value)
        {            
            foreach (var rule in _rules)
            {
                var result = rule.Validate(value);
                if (result.IsValid == false)
                {
                    return result;
                }
            }
            return new ValidationResult(true);
        }
    }
}
