namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;
    using System.Linq;

    public class NotDuplicateGatewayValidationRule : IValidationRule<string>
    {
        private readonly IModelCollection<GatewayModel> _gatewaysCollection;
        private readonly Guid _gatewayId;

        public NotDuplicateGatewayValidationRule(IModelCollection<GatewayModel> gatewaysCollection)
            : this(gatewaysCollection, Guid.Empty)
        { }

        public NotDuplicateGatewayValidationRule(IModelCollection<GatewayModel> gatewaysCollection, Guid gatewayId)
        {
            _gatewaysCollection = gatewaysCollection;
            _gatewayId = gatewayId;
        }

        //Return valid iff there are no saved credentials with a different Id but the same username (case insensitive)
        public IValidationResult Validate(string value)
        {
            StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Empty();
            }
            else if (_gatewaysCollection.Models.Any(c => c.Id != _gatewayId && comparer.Equals(c.Model.HostName, value)))
            {
                return ValidationResult.Invalid(HostnameValidationFailure.Duplicate);
            }
            else
            {
                return ValidationResult.Valid();
            }
        }
    }
}
