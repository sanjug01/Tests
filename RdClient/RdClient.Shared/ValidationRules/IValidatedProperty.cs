namespace RdClient.Shared.ValidationRules
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IValidatedProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
        bool EnableValidation { get; set; }
        IValidationResult State { get; }
    }
}
