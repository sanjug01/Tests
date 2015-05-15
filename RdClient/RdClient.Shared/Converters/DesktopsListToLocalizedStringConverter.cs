using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public class DesktopsListToLocalizedStringConverter: IValueConverter
    {                
        private IStringTable _localizedString;
        public const string itemSeparatorStringId = "Common_ListItemSeparator_String";
        public const string emptyDesktopListStringId = "DeleteDesktopsView-Message-NoDesktops";

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IList<IModelContainer<DesktopModel>> desktopModelList = value as IList<IModelContainer<DesktopModel>>;

            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (desktopModelList == null || desktopModelList.Count < 1)
            {
                return _localizedString.GetLocalizedString(emptyDesktopListStringId);
            }
            else
            {
                string separator = _localizedString.GetLocalizedString(itemSeparatorStringId);
                StringBuilder localizedList = new StringBuilder();
                for (int i = 0; i < desktopModelList.Count; i++)
                {
                    if (i > 0)
                    {
                        localizedList.Append(separator);
                    }
                    localizedList.Append(desktopModelList[i].Model.HostName);
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
