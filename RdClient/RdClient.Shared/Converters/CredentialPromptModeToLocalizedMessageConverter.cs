using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class CredentialPromptModeToLocalizedMessageConverter : IValueConverter
    {
        private static readonly Dictionary<CredentialPromptMode, string> _codeMap;
        
        private IStringTable _localizedString;

        static CredentialPromptModeToLocalizedMessageConverter()
        {
            _codeMap = new Dictionary<CredentialPromptMode, string>();
            _codeMap[CredentialPromptMode.FreshCredentialsNeeded] = "CredentialPrompt_Message_FreshCredsNeeded_String";
            _codeMap[CredentialPromptMode.InvalidCredentials] = "CredentialPrompt_Message_InvalidCredentials_String";
        }

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is CredentialPromptMode))
            {
                throw new ArgumentException();
            }
            else if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }  

            CredentialPromptMode mode = (CredentialPromptMode) value;
            string result = "";   
            if (_codeMap.ContainsKey(mode))
            {
                result = _localizedString.GetLocalizedString(_codeMap[mode]);
            }
            return result;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
