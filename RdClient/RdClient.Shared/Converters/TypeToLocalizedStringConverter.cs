namespace RdClient.Shared.Converters
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class TypeToLocalizedStringConverter : IValueConverter
    {
        public IStringTable LocalizedString { get; set; }

        public string GetKey(object value, string parameter = null)
        {
            string typeName = value.GetType().FullName.Split('.').Last();
            StringBuilder key = new StringBuilder();
            key.Append(typeName);

            if (value.GetType().GetTypeInfo().IsEnum)
            {
                key.Append("_");
                key.Append(value.ToString());
            }

            if (parameter != null)
            {
                key.Append("_").Append(parameter);
            }

            key.Append("_String");

            return key.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                return this.LocalizedString.GetLocalizedString(GetKey(value, parameter as string));
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
