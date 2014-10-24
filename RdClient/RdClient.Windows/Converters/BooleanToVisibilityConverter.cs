using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is bool);
            Contract.Requires(targetType.Equals(typeof(Visibility)));

            bool visible = (bool)value;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is Visibility);
            Contract.Requires(targetType.Equals(typeof(bool)));

            Visibility visibility = (Visibility) value;

            if (visibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
