using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public class TypeToLocalizedStringConverter : IValueConverter
    {
        public IStringTable LocalizedString { private get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string typeName = value.GetType().FullName.Split('.').Last();
            string key = typeName + "_" + value.ToString() + "_String";

            return this.LocalizedString.GetLocalizedString(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
