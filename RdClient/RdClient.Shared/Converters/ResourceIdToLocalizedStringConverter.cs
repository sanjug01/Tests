using RdClient.Shared.Helpers;
using System;
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

            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (resourceId == null)
            {
                throw new ArgumentException("value to convert must be a non-null string");
            }
            else
            {
                return _localizedString.GetLocalizedString(resourceId);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }    
}
