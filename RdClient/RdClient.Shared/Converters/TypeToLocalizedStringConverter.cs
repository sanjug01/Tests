using RdClient.Shared.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public class TypeToLocalizedStringConverter : IValueConverter
    {
        public IStringTable LocalizedString { get; set; }

        public string GetKey(object value)
        {
            string typeName = value.GetType().FullName.Split('.').Last();
            StringBuilder key = new StringBuilder();
            key.Append(typeName);

            if (value.GetType().GetTypeInfo().IsEnum)
            {
                key.Append("_");
                key.Append(value.ToString());
            }

            key.Append("_String");

            return key.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.LocalizedString.GetLocalizedString(GetKey(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
