using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    /// <summary>
    /// if string is not empy, should make the element invisible, visible otherwise
    /// this helps to add watermark text as hints for text fields not yet initialized.
    /// </summary>
    public class StringLengthToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is string);
            Contract.Requires(targetType.Equals(typeof(Visibility)));

            string stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //
            // There is no reverse translation; the converter can be used only one-way.
            //
            throw new NotImplementedException();
        }
    }
}
