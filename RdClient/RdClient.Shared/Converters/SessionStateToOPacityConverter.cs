namespace RdClient.Shared.Converters
{
    using RdClient.Shared.Models;
    using System;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter of SessionState enum values into rendering panel XAML opacity (0.0 - 1.0)
    /// </summary>
    public sealed class SessionStateToOpacityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (null == value)
                throw new ArgumentNullException("value");

            if (!(value is SessionState))
                throw new ArgumentException("Type of input value must be RdClient.Shared.Models.SessionState", "value");
            //
            // The rendering panel is opaque only in the Connected state. In all other states it is transparent.
            //
            double opacity = (SessionState)value == SessionState.Connected ? 1.0 : 0.0;

            return opacity;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
