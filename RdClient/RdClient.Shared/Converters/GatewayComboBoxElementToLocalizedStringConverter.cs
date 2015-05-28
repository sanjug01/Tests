using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public sealed class GatewayComboBoxElementToLocalizedStringConverter : IValueConverter
    {
        private static readonly Dictionary<GatewayComboBoxType, string> _codeMap;

        private IStringTable _localizedString;

        static GatewayComboBoxElementToLocalizedStringConverter()
        {
            _codeMap = new Dictionary<GatewayComboBoxType, string>();
            _codeMap[GatewayComboBoxType.AddNew] = "AddOrEditDesktop_GatewayOption_Add_String";
            _codeMap[GatewayComboBoxType.None] = "AddOrEditDesktop_GatewayOption_None_String";
        }

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            GatewayComboBoxElement comboBox = value as GatewayComboBoxElement;
            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (comboBox == null)
            {
                throw new ArgumentException("value to convert must be a non-null GatewayComboBoxElement");
            }
            else
            {
                string result;
                if (_codeMap.ContainsKey(comboBox.GatewayComboBoxType))
                {
                    result = _localizedString.GetLocalizedString(_codeMap[comboBox.GatewayComboBoxType]);
                }
                else if (GatewayComboBoxType.Gateway == comboBox.GatewayComboBoxType && comboBox.Gateway != null)
                {
                    result = comboBox.Gateway.Model.HostName;
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
