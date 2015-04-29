using RdClient.Shared.Data;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ValidationRules
{
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

        //Return valid iff there are no saved credentials with the same username but a different Id
        public IValidationResult Validate(string value)
        {
            StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
            bool valid = !_credCollection.Models.Any(c => c.Id != _credId && comparer.Equals(c.Model.Username, value));
            return new ValidationResult(valid, UsernameValidationFailure.Duplicate);
        }
    }
}
