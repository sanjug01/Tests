namespace RdClient.Shared.ValidationRules
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Windows.UI.Xaml;

    public interface IValidatedProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
        bool ShowErrors { get; set; }
        Visibility ErrorVisible { get; }
        IValidationResult State { get; }
    }
}
