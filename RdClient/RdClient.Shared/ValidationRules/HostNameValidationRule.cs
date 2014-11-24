using System.Globalization;

namespace RdClient.Shared.ValidationRules
{
    public class HostNameValidationRule : IllegalCharacterValidationRule
    {
        // list of illegal caracters - as for android
        public HostNameValidationRule() : base("`~!#@$%^&*()=+{}\\|;'\",< >/?")
        {
        }
    }
}
