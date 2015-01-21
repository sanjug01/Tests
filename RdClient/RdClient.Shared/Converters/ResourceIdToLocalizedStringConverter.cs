using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public sealed class ResourceIdToLocalizedStringConverter : IValueConverter
    {                
        private IStringTable _localizedString;

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string resourceId = value as string;

            if (resourceId == null || _localizedString == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                return _localizedString.GetLocalizedString(resourceId);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
}
