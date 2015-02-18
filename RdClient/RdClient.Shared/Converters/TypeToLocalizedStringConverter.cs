using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Reflection;

namespace RdClient.Shared.Converters
{
    public class TypeToLocalizedStringConverter : IValueConverter
    {
        public IStringTable LocalizedString { get; set; }

        public string GetKey(object value)
        {
            string typeName = value.GetType().FullName.Split('.').Last();
            string key = typeName;

            if (value.GetType().GetTypeInfo().IsEnum)
            {
                key += "_" + value.ToString();
            }

            key += "_String";

            return key;
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
