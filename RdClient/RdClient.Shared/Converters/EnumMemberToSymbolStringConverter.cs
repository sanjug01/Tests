namespace RdClient.Shared.Converters
{
    using System;
    using System.Reflection;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter that converts a member of any enum to a string consisting of one character with the unicode
    /// code point equal to the integer value of the enum member.
    /// </summary>
    public sealed class EnumMemberToSymbolStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (null == value)
                throw new ArgumentNullException("value");

            TypeInfo typeInfo = value.GetType().GetTypeInfo();

            if (!typeInfo.IsEnum)
                throw new ArgumentException("Input value must be a member of an enum", "value");

            return new string((char)Convert.ChangeType(value, typeof(char)), 1);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
