using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool b = (bool)value;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility v = (Visibility)value;
            return (Visibility.Visible != v);
        }
    }
}
