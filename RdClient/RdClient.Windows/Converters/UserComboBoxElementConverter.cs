using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class UserComboBoxElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is UserComboBoxElement);
            Contract.Requires(targetType.Equals(typeof(string)));

            UserComboBoxElement element = value as UserComboBoxElement;
            string description;

            switch(element.UserComboBoxType)
            {
                case UserComboBoxType.AddNew:
                    description = "Add New (d)";
                    break;
                case UserComboBoxType.AskEveryTime:
                    description = "Ask Every Time (d)";
                    break;
                case UserComboBoxType.Credentials:
                    description = element.Credentials.Username;
                    break;
                default:
                    description = "";
                    break;
            }

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
