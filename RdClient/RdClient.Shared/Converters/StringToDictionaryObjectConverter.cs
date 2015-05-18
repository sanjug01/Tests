namespace RdClient.Shared.Converters
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Generic converter that decfines a dictionary of objects keyed with strings and converts
    /// input value to an object keyed in the dictionary by the value's string representation.
    /// </summary>
    public sealed class StringToDictionaryObjectConverter : MutableObject, IValueConverter
    {
        private Dictionary<string, object> _dictionary;
        private string _defaultObject;

        /// <summary>
        /// Dictionary that may be set and populated in XAML.
        /// </summary>
        public Dictionary<string, object> Dictionary
        {
            get
            {
                if (null == _dictionary)
                    _dictionary = new Dictionary<string, object>();
                return _dictionary;
            }

            set
            {
                this.SetProperty<Dictionary<string, object>>(ref _dictionary, value);
            }
        }

        public string DefaultObject
        {
            get { return _defaultObject; }
            set { this.SetProperty<string>(ref _defaultObject, value); }
        }

        protected override void DisposeManagedState()
        {
            if (null != _dictionary)
            {
                _dictionary.Clear();
                _dictionary = null;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Ensures(null != Contract.Result<object>());

            object convertedValue;
            //
            // Null input value immediately falls back to the default dictionary object.
            //
            if(null == value || !_dictionary.TryGetValue(value.ToString(), out convertedValue) )
            {
                //
                // If the default key is specified, try to look up the default object.
                //
                if( null == _defaultObject || !_dictionary.TryGetValue(_defaultObject, out convertedValue) )
                {
                    throw new KeyNotFoundException(string.Format("Couldn't find object for key {0}", value));
                }
            }
            Contract.Assert(null != convertedValue);
            Type convertedType = convertedValue.GetType();
            //
            // Check if the requested type and the type of object found in the dictionary are compatible.
            //
            if (!targetType.GetTypeInfo().IsAssignableFrom(convertedType.GetTypeInfo()))
            {
                throw new InvalidCastException(string.Format("Requested target type {0}.{1} cannot be assigned from {2}.{3}",
                    targetType.Namespace, targetType.Name,
                    convertedType.Namespace, convertedType.Name));
            }

            return convertedValue;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //
            // There is no reverse translation; the converter is designed for use in one-way bindings.
            //
            throw new NotImplementedException();
        }
    }
}
