namespace RdClient.Shared.ValidationRules
{
    using System.ComponentModel;

    public interface IValidatedProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
        IValidationResult State { get; }
    }
}
