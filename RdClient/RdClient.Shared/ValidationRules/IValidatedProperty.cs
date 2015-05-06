namespace RdClient.Shared.ValidationRules
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IValidatedProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
        IValidationResult State { get; }
        bool ValidateNow();
    }
}
