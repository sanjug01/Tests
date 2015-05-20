namespace RdClient.Shared.ValidationRules
{
    public class HostnameValidationRule : CharacterOccurenceValidationRule
    {
        // list of illegal caracters - as for android
        public HostnameValidationRule() : base("`~!#@$%^&*()=+{}\\|;'\",< >/?", HostnameValidationFailure.InvalidCharacters)
        {
        }
    }
}
