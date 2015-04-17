namespace RdClient.Shared.Converters
{
    using System;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Value converter that negates the boolean value.
    /// </summary>
    public sealed class NegateBooleanConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
