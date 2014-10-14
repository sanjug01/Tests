namespace FadeTest
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
