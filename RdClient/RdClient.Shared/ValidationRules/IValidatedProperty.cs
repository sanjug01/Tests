namespace RdClient.Shared.ValidationRules
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IValidatedProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
        bool IsValid { get; }
        IEnumerable<object> Errors { get; }
    }
}
