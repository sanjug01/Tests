namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    /// <summary>
    /// Base class for all classes saved by the application's data model.
    /// </summary>
    [DataContract(IsReference=true)]
    public abstract class SerializableModel : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propertyChanged;

        protected SerializableModel()
        {
        }

        protected void SetProperty<TProperty>(ref TProperty property, TProperty newValue,
            [CallerMemberName] string propertyName = null) where TProperty : IEquatable<TProperty>
        {
            if(!property.Equals(newValue))
            {
                property = newValue;
                EmitPropertyChanged(propertyName);
            }
        }

        protected void EmitPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Contract.Assert(null != propertyName);

            PropertyChangedEventHandler handler = _propertyChanged;

            if (null != handler)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
    }
}
