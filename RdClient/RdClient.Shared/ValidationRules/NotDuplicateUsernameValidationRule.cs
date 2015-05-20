namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;
    using System.Linq;

    public class NotDuplicateUsernameValidationRule : IValidationRule<string>
    {
        private readonly IModelCollection<CredentialsModel> _credCollection;
        private readonly Guid _credId;

        public NotDuplicateUsernameValidationRule(IModelCollection<CredentialsModel> credCollection)
            : this(credCollection, Guid.Empty)
        { }

        public NotDuplicateUsernameValidationRule(IModelCollection<CredentialsModel> credCollection, Guid credId)
        {
            _credCollection = credCollection;
            _credId = credId;
        }

        //Return valid iff there are no saved credentials with a different Id but the same username (case insensitive)
        public IValidationResult Validate(string value)
        {
            StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
            if (string.IsNullOrEmpty(value))
            {
                return new ValidationResult(ValidationResultStatus.NullOrEmpty);
            }
            else if (_credCollection.Models.Any(c => c.Id != _credId && comparer.Equals(c.Model.Username, value)))
            {
                return new ValidationResult(ValidationResultStatus.Invalid, UsernameValidationFailure.Duplicate);
            }
            else
            {
                return new ValidationResult(ValidationResultStatus.Valid);
            }
        }
    }
}
