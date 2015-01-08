using System;
namespace RdClient.Shared.ValidationRules
{
    public interface IValidationRule
    {
        bool Validate(object value, System.Globalization.CultureInfo cultureInfo);
    }
}
