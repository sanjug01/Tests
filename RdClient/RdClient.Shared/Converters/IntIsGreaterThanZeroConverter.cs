namespace RdClient.Shared.Converters
{
    using System;
    using System.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Returns true iff value is a non-empty ICollection
    /// Returns false if value is an empty ICollection
    /// Returns DependencyProperty.UnsetValue if value is not an ICollection
    /// </summary>
    public sealed class IntIsGreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var number = value as int?;
            if (number != null)
            {
                return number > 0;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
