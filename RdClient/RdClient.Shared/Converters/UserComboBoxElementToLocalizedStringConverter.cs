using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public sealed class UserComboBoxElementToLocalizedStringConverter : IValueConverter
    {
        private static readonly Dictionary<UserComboBoxType, string> _codeMap;

        private IStringTable _localizedString;

        static UserComboBoxElementToLocalizedStringConverter()
        {
            _codeMap = new Dictionary<UserComboBoxType, string>();
            _codeMap[UserComboBoxType.AddNew] = "AddOrEditDesktop_CredOption_Add_String";
            _codeMap[UserComboBoxType.AskEveryTime] = "AddOrEditDesktop_CredOption_Ask_String";
        }

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            UserComboBoxElement comboBox = value as UserComboBoxElement;
            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (comboBox == null)
            {
                throw new ArgumentException("value to convert must be a non-null UserComboBoxElement");
            }
            else
            {
                string result;
                if (_codeMap.ContainsKey(comboBox.UserComboBoxType))
                {
                    result = _localizedString.GetLocalizedString(_codeMap[comboBox.UserComboBoxType]);
                }
                else if (UserComboBoxType.Credentials == comboBox.UserComboBoxType && comboBox.Credentials != null)
                {
                    result = comboBox.Credentials.Model.Username;
                }
                else
                {
                    result = "";
                }
                return result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
