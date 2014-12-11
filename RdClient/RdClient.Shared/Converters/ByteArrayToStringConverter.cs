namespace RdClient.Converters
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Generic converter to extract a byte array to a string, using hex represantation
    /// </summary>
    public sealed class ByteArrayToStringConverter : MutableObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is byte[]);
            Contract.Requires(targetType.Equals(typeof(string)));

            byte[] bytes = value as byte[];
            return BitConverter.ToString(bytes); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //
            // There is no reverse translation; the converter is designed for use in one-way bindings.
            //
            throw new NotImplementedException();
        }
    }
}
