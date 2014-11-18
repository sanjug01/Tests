using System.Globalization;

namespace RdClient.Shared.ValidationRules
{
    public class HostNameValidationRule
    {
        // list of illegal caracters - as for android
        private const string _illegalCharacters = "`~!#@$%^&*()=+{}\\|;'\",< >/?";

        public static bool Validate(object value, CultureInfo cultureInfo)
        {
            string stringValue = value as string;

            if(!string.IsNullOrEmpty(stringValue))
            {
                if(HostNameValidationRule.hasIllegalCharacters(stringValue))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool hasIllegalCharacters(string hostName)
        {
            for (int i = 0; i < _illegalCharacters.Length; i++ )
            {
                if(hostName.IndexOf(_illegalCharacters[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
