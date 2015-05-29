namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;
    using System.Linq;
    using System.Reflection;

    public class NotDuplicateValidationRule<TModel> : IValidationRule<string>
        where TModel : class, IPersistentStatus
    {
        private readonly IModelCollection<TModel> _modelCollection;
        private readonly Guid _modelId;
        private readonly IModelEqualityComparer<TModel, string> _comparer;
        private readonly object _invalidResult;

        public NotDuplicateValidationRule(IModelCollection<TModel> collection, IModelEqualityComparer<TModel, string> comparer, object invalidResult)
            : this(collection, Guid.Empty, comparer, invalidResult)
        { }

        public NotDuplicateValidationRule(IModelCollection<TModel> collection, Guid modelId, IModelEqualityComparer<TModel, string> comparer, object invalidResult)
        {
            _modelCollection = collection;
            _modelId = modelId;
            _invalidResult = invalidResult;
            _comparer = comparer;
        }

        // Return valid iff there is no saved model with a different Id but the same name 
        public IValidationResult Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Empty();
            }
            else if ( _modelCollection.Models.Any(c => c.Id != _modelId && _comparer.Equals(c.Model, value)) )
            {
                return ValidationResult.Invalid(_invalidResult);
            }
            else
            {
                return ValidationResult.Valid();
            }
        }
    }
}
