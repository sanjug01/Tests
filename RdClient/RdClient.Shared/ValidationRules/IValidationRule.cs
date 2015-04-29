using System;
namespace RdClient.Shared.ValidationRules
{
    public interface IValidationRule<T>
    {
        IValidationResult Validate(T value);
    }
}
