using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class DesktopsListToLocalizedStringConverter: IValueConverter
    {                
        private IStringTable _localizedString;
        public const string itemSeparatorStringId = "Common_ListItemSeparator_String";
        public const string emptyDesktopListStringId = "DeleteDesktopsView-Message-NoDesktops";

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IList<Desktop> desktopList = value as IList<Desktop>;
            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (desktopList == null || desktopList.Count < 1)
            {
                return _localizedString.GetLocalizedString(emptyDesktopListStringId);
            }
            else
            {
                string separator = _localizedString.GetLocalizedString(itemSeparatorStringId);
                StringBuilder localizedList = new StringBuilder();
                for (int i = 0; i < desktopList.Count; i++)
                {
                    if (i > 0)
                    {
                        localizedList.Append(separator);
                    }
                    localizedList.Append(desktopList[i].HostName);
                }
                return localizedList.ToString();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
