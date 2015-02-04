using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class ReconnectAttemptsToLocalizedStringConverter : IValueConverter
    {                
        private IStringTable _localizedString;
        public const string reconnectAttemptsStringId = "Session_Reconnect_Attempts_String";

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else if (value == null || !(value is  int))
            {
                throw new ArgumentException("value to convert must be an int");
            }
            else
            {
                int countAttempts = (int) value;
                string result = String.Format(
                    _localizedString.GetLocalizedString(reconnectAttemptsStringId),
                    countAttempts.ToString(), SessionModel.MaxReconnectAttempts 
                    );
                return result;         
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
